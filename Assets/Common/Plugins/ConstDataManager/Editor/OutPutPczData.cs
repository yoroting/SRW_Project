using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

using System.Text;
using Playcoo.Common;

public class OutPutPczData {
	[MenuItem("Custom/建立 ConstData 對應的 Class")]
	public static void OutPutPCZClass()
	{
		//預設 pcz 資料夾
		string path = Application.dataPath + "/StreamingAssets/pcz/";
		//輸出資料夾
		string outputPath = Application.dataPath+"/_Project/Script/ConstDataClasses";
		
		string fileName = EditorUtility.OpenFilePanel(
                "Output PCZ Class",
                path,
                "pcz");
		
		path = Path.GetDirectoryName(fileName);

		Debug.Log(fileName);
		ConstDataManager manager = ConstDataManager.Instance;
		manager.isLazyMode = false;
		manager.useUnregistedTables = true;
		manager.ReadData(fileName);

		StringBuilder tableBuilder = new StringBuilder();
		StringBuilder registerBuilder = new StringBuilder();
		
//		UnityEditor.EditorUtility.DisplayProgressBar(
//			"還原遊戲中修改到的 Material",
//			"進度: " + i + "/" + totalCount,
//			(float)i/(float)totalCount);
//		UnityEditor.EditorUtility.ClearProgressBar();
		foreach(DataTable table in manager.ListTables())
		{
			string classStr = "";
			string tableName = "";
			OutputDataTableClass(table, out tableName, out classStr);
			
			string classFileName = outputPath +"/" +tableName +".cs";
			File.WriteAllText(classFileName, classStr, Encoding.UTF8);
			Debug.Log("轉換完成, name =\t" + table.Name + "\n");

			tableBuilder.AppendLine(string.Format("\t{0} = {1},", table.Name, table.TableID));
			registerBuilder.AppendLine(string.Format("ConstDataReader.Instance.RegisterTableType<{0}>((int)ConstDataTables.{1});", table.Name, table.Name));
		}

		Debug.Log(tableBuilder.ToString());
		Debug.Log(registerBuilder.ToString());

		AssetDatabase.Refresh();
	}

	private static void OutputDataTableClass(DataTable table, out string tableName, out string classStr)
	{
		StringBuilder sb = new StringBuilder();

		sb.AppendLine("using Playcoo.Common;");
		sb.AppendLine(string.Format("public class {0} : ConstDataRow<{0}>", table.Name));
		sb.AppendLine("{");
		sb.AppendLine(string.Format("\tpublic const int TableID = {0};", table.TableID));
		sb.AppendLine(string.Format("\tpublic const int DigitBase = {0};", table.DigitBase));


		string fileformatStr = "\tpublic {1} {0};";

		foreach (var column in table.Columns)
		{
			string typeName = "";

			switch (column.DataType)
			{
				case ConstDataType.Int:
				case ConstDataType.BigInt:
				{
					typeName = "int";
					break;
				}
				case ConstDataType.Float:
				{
					typeName += "float";
					break;
				}
				default:
				{
					typeName = "string";
					break;
				}
			}

			sb.AppendLine(string.Format(fileformatStr, column.Name, typeName));
		}
		sb.AppendLine("}");
		
		classStr = sb.ToString();
		tableName = table.Name;
	}
}
