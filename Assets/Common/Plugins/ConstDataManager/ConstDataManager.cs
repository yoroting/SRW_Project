using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Collections;
using Playcoo.Common;

public enum ConstDataType
{
	Int,
	BigInt,
	Float,
	String,
}

/// <summary>
/// Const data manager.
/// 
/// 讀值方法
/// 
/// DataRow row = ConstDataManager.Instance.GetRow("UI_MESSAGE", 34); 	
/// if(row != null)	
///		Debug.Log(row.Field<string>("s_UI_WORDS"));
///
/// 
/// 
/// AREA_NAME pvpRow = ConstDataManager.Instance.GetRow<AREA_NAME>(1);
/// Debug.Log(pvpRow.s_NAME);
/// 
/// </summary>


public partial class ConstDataManager
{
	public const int RecordIDUpperValue = 100000;

	/// <summary>
	/// 延遲生成模式，設為true時讀取時不解析資料表，改在第一次讀表時解析
	/// </summary>
	public bool isLazyMode = false;

	/// <summary>
	/// 是否使用未註冊的資料表，設為true時未註冊的資料會讀成DefaultDataRow
	/// </summary>
	public bool useUnregistedTables = true;

	private static ConstDataManager instance;

	private Dictionary<int, DataTable> idToTable;

	private Dictionary<string, DataTable> nameToTable;

	private Dictionary<int, Type> tableTypes = new Dictionary<int, Type>();
	private Dictionary<Type, int> map_type_tableID = new Dictionary<Type, int>();

	private Dictionary<int, byte[]> tableSources;

	public static ConstDataManager Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new ConstDataManager();
				instance.Clear();
			}

			return instance;
		}
	}

	protected ConstDataManager() {}

	public void Clear()
	{
		idToTable = null;
		nameToTable = null;
		tableSources = null;
	}

	public void CleanRegistedTables()
	{
		tableTypes.Clear();
		map_type_tableID.Clear();
	}

	public List<DataTable> ListTables()
	{
		List<DataTable> list = new List<DataTable>(idToTable.Count);

		if (idToTable != null)
		{
			foreach (var pair in idToTable)
			{
				list.Add(pair.Value);

				if (isLazyMode)
					CheckTableIsReadable(pair.Value);
			}
		}

		return list;
	}

//	/// <summary>初始化</summary>
//	public void Init(){
//			//用反射註冊所有對應 Table 的 Class
//			Assembly asm = Assembly.GetExecutingAssembly();
//			
////			List<string> namespacelist = new List<string>();
////			List<string> classlist = new List<string>();
//
//			Type[] types = asm.GetTypes();
//			foreach (Type type in types)
//			{
//				if(!(type is DataRow))
//					continue;
//				int a=0;
//				Debug.Log("aaaa");
////				if (type.Namespace == nameSpace)
////					namespacelist.Add(type.Name);
//			}
//			
////			foreach (string classname in namespacelist)
////				classlist.Add(classname);
//	}

    /// <summary>
    /// 讀取 StreamingAssets 中的 pcz 檔, 不使用IEnumerator
    /// </summary>
    /// <param name="dataPathRelativeAssets">檔案相對於 Assets 的路徑 ex: "_Project/StreamingAssets/pcz/"</param>
    /// <param name="dataNames">Data names.</param>
    public void NormalReadDataStreaming(string dataPathRelativeAssets, string[] dataNames)
    {
        for (int i = 0; i < dataNames.Length; i++)
        {
            string rootPath = null;

            rootPath = "file://" + Application.dataPath + "/StreamingAssets/" + dataPathRelativeAssets + dataNames[i] + ".pcz";
            WWW www = new WWW(rootPath);
            ReadData(www.bytes);
            while (!www.isDone)
            {
            }
        }
    }

	/// <summary>
	/// 讀取 StreamingAssets 中的 pcz 檔
	/// </summary>
	/// <param name="dataPathRelativeAssets">檔案相對於 Assets 的路徑 ex: "_Project/StreamingAssets/pcz/"</param>
	/// <param name="dataNames">Data names.</param>
	/// <param name="endFunc">End func.</param>
	public IEnumerator ReadDataStreaming(string dataPathRelativeAssets, string[] dataNames, Action endFunc=null)
	{
		for (int i = 0; i < dataNames.Length; i++) {
			string rootPath = null;

			#if UNITY_EDITOR                        
			rootPath = "file://" + Application.dataPath + "/StreamingAssets/" + dataPathRelativeAssets + dataNames[i] + ".pcz";
			#elif UNITY_IPHONE
			rootPath = "file://" + Application.dataPath + "/Raw/" + dataNames[i] + ".pcz";
			#elif UNITY_ANDROID
			rootPath = "jar:file://" + Application.dataPath + "!/assets/" + dataPathRelativeAssets + dataNames[i] + ".pcz";
			#else
			rootPath = "file://" + Application.dataPath + "/StreamingAssets/" + dataPathRelativeAssets + dataNames[i] + ".pcz";
			#endif


			WWW www = new WWW(rootPath);
			if(endFunc != null){
				yield return www;
			}else{
				while(!www.isDone){

				}
			}

			ReadData(www.bytes);
		}

		if(endFunc != null)
			endFunc();
	}

	/// <summary>
	/// 讀取二進位檔
	/// </summary>
	/// <param name="bytes">Bytes.</param>
	public void ReadData(byte[] bytes)
	{
		ReadStream(new MemoryStream(bytes));
	}

	/// <summary>
	/// 讀取路徑
	/// </summary>
	/// <param name="Path">Path.</param>
	public void ReadData(string path)
	{
		ReadStream(new FileStream(path, FileMode.Open, FileAccess.Read));
	}

	/// <summary>
	/// 非同步讀取資料
	/// </summary>
	/// <returns>The data async.</returns>
	/// <param name="bytes">Bytes.</param>
	public IEnumerator ReadDataAsync(byte[] bytes, Action callback)
	{
		using (BinaryReader br = new BinaryReader(new MemoryStream(bytes)))
		{
			int tableSize = br.ReadInt32();
			for (int i = 0; i < tableSize; ++i)
			{
				ReadDataTable(br);

				yield return 0;
			}
		}
		if (callback != null)
			callback();
	}

	private void ReadStream(Stream stream)
	{
		using (BinaryReader br = new BinaryReader(stream))
        {
            int tableSize = br.ReadInt32();
            for (int i = 0; i < tableSize; ++i)
            {
				ReadDataTable(br);
            }
        }
	}

	private void ReadDataTable(BinaryReader br)
	{
		//讀取Table資料
		string tableName = ReadString(br);
		int tableID = br.ReadInt32();
		int digitBase = br.ReadInt32();
		string refTableName = ReadString(br);
		
		DataTable table = new DataTable(tableID, tableName, digitBase, refTableName);
		
		// 讀取 Column 資料
		int columnSize = br.ReadInt32();
		
		for (int j = 0; j < columnSize; ++j)
		{
			string columnName = ReadString(br);
			ConstDataType dataType = (ConstDataType)br.ReadInt32();
			//Size
			br.ReadInt32();
			//dataOffset
			br.ReadInt32();
			
			table.AddColumn(columnName, dataType);
		}
		
		if (!isLazyMode)
		{
			if (useUnregistedTables || tableTypes.ContainsKey(table.TableID))
			{
				ReadTableRows(table, br);
			}
			else
			{
				//謹讀取，不儲存
				ReadTableBytes(table, br);
			}
		}
		else
		{
			if (tableSources == null)
				tableSources = new Dictionary<int, byte[]>();
			
			byte[] source = ReadTableBytes(table, br);
			
			if (useUnregistedTables || tableTypes.ContainsKey(table.TableID))
				tableSources.Add(table.TableID, source);//LazyMode時僅儲存二進位資料
		}
		
		if (idToTable == null || nameToTable == null)
		{
			idToTable = new Dictionary<int, DataTable>();
			nameToTable = new Dictionary<string, DataTable>();
		}
		
		if (!idToTable.ContainsKey(table.TableID) && !nameToTable.ContainsKey(table.Name))
		{
			idToTable.Add(table.TableID, table);
			nameToTable.Add(table.Name, table);
		}
		else
		{
			idToTable[table.TableID] = table;
			nameToTable[table.Name] = table;
			Debug.LogWarning(string.Format("覆蓋相同的ConstDataTable, id={0}, name={1}", table.TableID, table.Name));
		}
	}

	/// <summary>
	/// 取得列的固定大小(byte)，回傳-1表示為動態大小
	/// </summary>
	/// <returns>The row size.</returns>
	/// <param name="table">Table.</param>
	private int GetRowSize(DataTable table)
	{
		int size = 0;

		foreach (ConstDataColumn column in table.Columns)
		{
			if (column.DataType == ConstDataType.String)
				return -1;

			if (column.DataType == ConstDataType.BigInt)
				size += 8;
			else
				size += 4;
		}
		return size;
	}

	private byte[] ReadTableBytes(DataTable table, BinaryReader br)
	{
		List<byte> bytes = new List<byte>();

		int rowNum = br.ReadInt32();

		bytes.AddRange(BitConverter.GetBytes(rowNum));

		int rowSize = GetRowSize(table);

		if (rowSize != -1)
		{
			//固定大小，直接算乘積
			int len = rowSize * rowNum;
			bytes.AddRange(br.ReadBytes(len));
		}
		else
		{
			//動態大小
			for (int j = 0; j < rowNum; ++j)
			{
				foreach (ConstDataColumn column in table.Columns)
				{
					switch (column.DataType)
					{
					case ConstDataType.Int: bytes.AddRange(br.ReadBytes(4)); break;
					case ConstDataType.BigInt: bytes.AddRange(br.ReadBytes(8)); break;
					case ConstDataType.Float: bytes.AddRange(br.ReadBytes(4)); break;
					case ConstDataType.String:
					{
						int length = br.ReadInt32();
						
						bytes.AddRange(BitConverter.GetBytes(length));
						bytes.AddRange(br.ReadBytes(length));
						break;
					}
					}
				}
			}
		}

		return bytes.ToArray();
	}

	private void ReadTableRows(DataTable table, byte[] bytes)
	{
//			#if UNITY_EDITOR
//			float time = Time.realtimeSinceStartup;
//			#endif
		using (BinaryReader br = new BinaryReader(new MemoryStream(bytes)))
		{
			ReadTableRows(table, br);
		}
//#if UNITY_EDITOR
//			Debug.Log("解析ConstData, name=" + table.Name + ", cost=" + (Time.realtimeSinceStartup - time));
//#endif
	}

	private void ReadTableRows(DataTable table, BinaryReader br)
	{
		Type targetType = null;

		if(!tableTypes.TryGetValue(table.TableID, out targetType)){
			targetType = Type.GetType(table.Name);
			if(targetType != null){
				tableTypes.Add(table.TableID, targetType);
				map_type_tableID.Add(targetType, table.TableID);
			}
		}

		if (targetType == null)
		{
			Debug.LogWarning("沒有對應 ConstDataTabel 的 Class, 改用預設(DefaultDataRow), table=" + table.Name);
			targetType = typeof(DefaultDataRow);
		}

		// 讀取所有的 Row
		int rowSize = br.ReadInt32();
		
		for (int j = 0; j < rowSize; ++j)
		{
			//建立對應類別
			DataRow row = (DataRow)Activator.CreateInstance(targetType);
			
			foreach (ConstDataColumn column in table.Columns)
			{
				if (row.ContainField(column.Name))
				{
					//賦值
					switch (column.DataType)
					{
						case ConstDataType.Int:
						{
							row[column.Name] = br.ReadInt32();
							break;
						}
						case ConstDataType.BigInt:
						{
							row[column.Name] = br.ReadInt64(); 
							break;
						}
						case ConstDataType.Float: 
						{
							row[column.Name] = br.ReadSingle();
							break;
						}
						case ConstDataType.String:
						{
							row[column.Name] = ReadString(br); 
							break;
						}
					}
				}
				else
				{
					if (j == 0)
						Debug.LogWarning(string.Format("設定DataRow欄位失敗，沒有對應的欄位, table={0}, column={1}", table.Name, column.Name));
					
					//ReadNext
					switch (column.DataType)
					{
						case ConstDataType.Int: br.ReadInt32(); break;
						case ConstDataType.BigInt: br.ReadInt64(); break;
						case ConstDataType.Float: br.ReadSingle(); break;
						case ConstDataType.String: ReadString(br); break;
					}
				}
			}
			
			table.AddRow(row);
		}
	}

	/// <summary>
	/// Gets a value indicating whether data have import.
	/// </summary>
	/// <value><c>true</c> if have read data; otherwise, <c>false</c>.</value>
	public bool HaveReadData
	{
		get{ return (idToTable != null) && (nameToTable != null); }
	}

//		/// <summary>
//		/// 註冊ConstDataTable的資料類型
//		/// </summary>
//		/// <param name="tableID">Table I.</param>
//		/// <typeparam name="T">The 1st type parameter.</typeparam>
//		public void RegisterTableType<T>(int tableID) where T : DataRow
//		{
//			if (tableTypes == null)
//				tableTypes = new Dictionary<int, Type>();
//			
//			if (!tableTypes.ContainsKey(tableID))
//			{
//				tableTypes.Add(tableID, typeof(T));
//			}
//			else
//			{
//				Debug.Log("註冊失敗，有相同的tableID");
//			}
//			
//		}

	/// <summary>
	/// 檢查表格是否可讀取，不可讀時自動載入
	/// </summary>
	/// <param name="table">Table.</param>
	private void CheckTableIsReadable(DataTable table)
	{
		if (tableSources == null || table == null || table.IsReadable)
			return;

		//從二進位檔讀入
		byte[] bytes = null;

		tableSources.TryGetValue(table.TableID, out bytes);

		if (bytes != null)
		{
			ReadTableRows(table, bytes);

			//移除bytes
			tableSources.Remove(table.TableID);
		}
	}

	public DataTable GetTable<T>() where T:DataRow
	{
		int tableID = map_type_tableID[typeof(T)];
		return GetTable(tableID);
	}
	public DataTable GetTable(int tableID)
	{
		if (idToTable == null)
			return null;

		DataTable table = null;

		idToTable.TryGetValue(tableID, out table);

		if (isLazyMode)
			CheckTableIsReadable(table);

		return table;
	}

	public DataTable GetTable(string tableName)
	{
		if (nameToTable == null)
			return null;

		DataTable table = null;
		
		nameToTable.TryGetValue(tableName, out table);

		if (isLazyMode)
			CheckTableIsReadable(table);

		return table;
	}
	
	public DataRow GetRow(string tableName, int halfID)
	{
		DataTable table = GetTable(tableName);
		return table != null ? table.GetRow<DataRow>(halfID) : null;
	}

	public DataRow GetRow(int tableID, int halfID)
	{
		DataTable table = GetTable(tableID);
		return (table != null) ? table.GetRow<DataRow>(halfID) : null;
	}
	public DataRow GetRow(int tableID, string column, object value)
	{
		DataTable table = GetTable(tableID);
		return table != null ? table.GetRow<DataRow>(column, value) : null;
	}
	public DataRow GetRow(int tableID, string[] columns, object[] values)
	{
		DataTable table = GetTable(tableID);
		return table != null ? table.GetRow<DataRow>(columns, values) : null;
	}

	public T GetRow<T>(int halfID) where T : DataRow
	{
		DataTable table = GetTable<T>();
		return table != null ? table.GetRow<T>(halfID) : null;
	}
	public T GetRow<T>(string column, object value) where T : DataRow
	{
		DataTable table = GetTable<T>();
		return table != null ? table.GetRow<T>(column, value) : null;
	}
	public T GetRow<T>(string[] columns, object[] values) where T : DataRow
	{
		DataTable table = GetTable<T>();
		return table != null ? table.GetRow<T>(columns, values) : null;
	}
	
//		public T GetRow<T>(string tableName, int id) where T : DataRow
//		{
//			DataTable table = GetTable(tableName);
//			return table != null ? table.GetRow<T>(id) : null;
//		}
//		
//		public T GetRow<T>(string tableName, string column, object value) where T : DataRow
//		{
//			DataTable table = GetTable<T>(tableName);
//			return table != null ? table.GetRow<T>(column, value) : null;
//		}
//
//		public T GetRow<T>(string tableName, string[] columns, object[] values) where T : DataRow
//		{
//			DataTable table = GetTable(tableName);
//			return table != null ? table.GetRow<T>(columns, values) : null;
//		}

	public T[] GetRows<T>(string columnName, object value) where T : DataRow
	{
		DataTable table = GetTable<T>();
		return table != null ? table.GetRows<T>(columnName, value) : null;
	}

	public T[] GetRows<T>(string[] columnNames, object[] values) where T : DataRow
	{
		DataTable table = GetTable<T>();
		return table != null ? table.GetRows<T>(columnNames, values) : null;
	}

	public T GetRowByFullID<T> (int fullID) where T : DataRow
	{
		int halfID = fullID % RecordIDUpperValue;
		return GetRow<T>(halfID);
	}
	public DataRow GetRowByFullID (int fullID)
	{
		int tableID = fullID / RecordIDUpperValue;
		int halfID = fullID % RecordIDUpperValue;
		return GetRow(tableID, halfID);
	}

	private string ReadString(BinaryReader br)
	{
		int length = br.ReadInt32();
		return System.Text.Encoding.Unicode.GetString(br.ReadBytes(length));
	}

}

public abstract class DataRow
{
	abstract public object this[string name]{get; set;}

	abstract public bool ContainField(string fieldName);

	/// <summary>
	/// 取得指定型態欄位資料
	/// </summary>
	/// <param name="columnName">Column name.</param>
	/// <typeparam name="U">The 1st type parameter.</typeparam>
	public T Field<T>(string columnName)
	{
		try
		{
			return (T)this[columnName];
		}
		catch
		{
			return default(T);
		}
	}
}

/// <summary>
/// 資料表
/// </summary>
public class DataTable : IEnumerable
{
	public const string IDENTIFY_COLUMN_NAME = "n_ID";

	private string tableName;

	private int tableID;

	private int digitBase;

	private string refTableName;

	private Dictionary<int, DataRow> rows;

	private List<DataRow> rowList;

	private List<ConstDataColumn> columns;

	public string Name
	{
		get { return tableName; }
	}
	public int TableID
	{
		get { return tableID; }
	}
	/// <summary>HalfID 轉 FullID 的位數定義，Excel 中欄位在 TableID 後面</summary>
	public int DigitBase
	{
		get { return digitBase; }
	}
	/// <summary>ConstData 對應到 TextData 中的 TableID，Excel 中欄位在 DigitBase 後面</summary>
	public string RefTableName
	{
		get { return refTableName; }
	}

	public List<ConstDataColumn> Columns
	{
		get {return columns;}
	}

	public List<DataRow> RowList
	{
		get {return rowList;}
	}

	public bool IsReadable
	{
		get {return rowList != null;}
	}

	public Dictionary<int, DataRow> Rows 
	{
		get {return rows;}
	}

	public IEnumerator GetEnumerator ()
	{
		return rowList.GetEnumerator();
	}

	public List<T> ListRows<T>() where T : DataRow
	{
		List<T> list = new List<T>();

		if (rowList.Count <= 0 || !(rowList[0] is T))
			return list;

		foreach (var data in rowList)
		{
			//轉型
			list.Add((T)data);
		}
		return list;
	}

	public void AddColumn(string columnName, ConstDataType dataType)
	{
		if (columns == null)
			columns = new List<ConstDataColumn>();

		ConstDataColumn column = new ConstDataColumn(columnName, dataType);

		columns.Add(column);
	}

	public DataTable(int tableID, string tableName, int digitBase, string refTableName)
	{
		this.tableID = tableID;
		this.tableName = tableName;
		this.digitBase = digitBase;
		this.refTableName = refTableName;
	}

	internal void AddRow(DataRow row)
	{
		if (rows == null || rowList == null)
		{
			rows = new Dictionary<int, DataRow>();
			rowList = new List<DataRow>();
		}

		int id = row.Field<int>(IDENTIFY_COLUMN_NAME);

		if (rows.ContainsKey(id))
		{
			Debug.LogWarning("加入相同id的Row, id=" + id);
			return;
		}

		rows.Add(id, row);
		rowList.Add(row);
	}

	public T GetRow<T>(int id) where T : DataRow
	{
		DataRow value = null;

		rows.TryGetValue(id, out value);

		return value as T;
	}

	public T GetRow<T>(string columnName, object value) where T : DataRow
	{
		if (!ContainColumn(columnName))
			return null;

		//取得欄位是id時，直接使用Dictionary
		if (columnName == IDENTIFY_COLUMN_NAME)
			return GetRow<T>(Convert.ToInt32(value));

		foreach (var pair in rows)
		{
			if (pair.Value[columnName].Equals(value))
				return pair.Value as T;
		}
		return null;
	}

	public T GetRow<T>(string[] columnNames, object[] values) where T : DataRow
	{
		if (columnNames.Length != values.Length)
			return null;
		
		if (rowList.Count <= 0 || !(rowList[0] is T))
			return null;
		
		int num = rowList.Count;
		int columnNum = columnNames.Length;
		
		for (int i = 0; i < num; ++i)
		{
			DataRow row = rowList[i];
			bool isEqual = true;
			for (int j = 0; j < columnNum; ++j)
			{
				if (!row[columnNames[j]].Equals(values[j]))
				{
					isEqual = false;
					break;
				}
			}
			if (isEqual)
				return row as T;
		}
		return null;
	}

	public bool ContainColumn(string columnName)
	{
		if (rowList == null || rowList.Count == 0)
			return false;

		return rowList[0].ContainField(columnName);
	}

	public T[] GetRows<T>(string columnName, object value) where T : DataRow
	{
		if (!ContainColumn(columnName))
			return null;

		if (rowList.Count <= 0 || !(rowList[0] is T))
			return null;

		List<T> result = new List<T>();

		int num = rowList.Count;

		for (int i = 0; i < num; ++i)
		{
			DataRow row = rowList[i];

			if (row[columnName].Equals(value))
			{
				result.Add((T)row);
			}
		}
		return result.ToArray();
	}

	public T[] GetRows<T>(string[] columnNames, object[] values) where T : DataRow
	{
		if (columnNames.Length != values.Length)
			return null;

		if (rowList.Count <= 0 || !(rowList[0] is T))
			return null;
		
		List<T> result = new List<T>();
		
		int num = rowList.Count;
		int columnNum = columnNames.Length;
		
		for (int i = 0; i < num; ++i)
		{
			DataRow row = rowList[i];

			bool isEqual = true;
			for (int j = 0; j < columnNum; ++j)
			{
				if (!row[columnNames[j]].Equals(values[j]))
				{
					isEqual = false;
					break;
				}
			}
			if (isEqual)
				result.Add((T)row);
		}
		return result.ToArray();
	}
}

/// <summary>
/// 指定型態的資料類型
/// </summary>
public class ConstDataRow<T> : DataRow
{
	private static Dictionary<string, FieldInfo> fieldInfos;

	/// <summary>
	/// 欄位資訊
	/// </summary>
	/// <value>The field infos.</value>
	public static Dictionary<string, FieldInfo> FieldInfos
	{
		get
		{
			if (fieldInfos == null)
			{
				fieldInfos = new Dictionary<string, FieldInfo>();
				
				Type type = typeof(T);
				
				FieldInfo[] targetFields = type.GetFields();
				
				//列表轉Dictionary
				foreach (var field in targetFields)
				{
					fieldInfos.Add(field.Name, field);
				}
			}
			return fieldInfos;
		}
	}
	
	/// <summary>
	/// 欄位是否存在
	/// </summary>
	/// <returns><c>true</c>, if field was contained, <c>false</c> otherwise.</returns>
	/// <param name="columnName">field name.</param>
	public override bool ContainField(string fieldName)
	{
		return FieldInfos.ContainsKey(fieldName);
	}
	
	/// <summary>
	/// 取得欄位資料
	/// </summary>
	/// <param name="columnName">Column name.</param>
	public override object this[string columnName]
	{
		get 
		{
			FieldInfo info = null;
			
			FieldInfos.TryGetValue(columnName, out info);
			
			return info != null ? info.GetValue(this) : null;
		}
		
		set
		{
			FieldInfo info = null;
			
			FieldInfos.TryGetValue(columnName, out info);
			
			if (info != null)
				info.SetValue(this, value);
		}
	}
}

/// <summary>
/// 預設的資料類型
/// </summary>
public class DefaultDataRow : DataRow
{
	private Dictionary<string, object> columnDatas;

	public override bool ContainField (string fieldName)
	{
		//不限制欄位
		return true;
	}

	public override object this [string name] 
	{
		get	
		{
			if (columnDatas == null)
				return null;
			object result = null;
			columnDatas.TryGetValue(name, out result);
			
			return result;
		}
		set 
		{
			if (columnDatas == null)
				columnDatas = new Dictionary<string, object>();

			if (!columnDatas.ContainsKey(name))
				columnDatas.Add(name, value);
			else
				columnDatas[name] = value;
		}
	}
}

/// <summary>
/// 資料欄位資訊
/// </summary>
public class ConstDataColumn
{
	private string columnName;
	private ConstDataType dataType;

	public string Name
	{
		get { return columnName; }
	}

	public ConstDataType DataType
	{
		get { return dataType; }
	}

	public ConstDataColumn(string columnName, ConstDataType dataType)
	{
		this.columnName = columnName;
		this.dataType = dataType;
	}
}



