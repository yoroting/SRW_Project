using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using _SRW;
using MYGRIDS;
// All SRW enum list
/// <summary>預設存在的 Channel Type</summary>
public enum _ACTION
{
	_WAIT = 0,
	_MOVE,
	_ATK,
	_DEF,
	_CAST,
	_Hited,
	_Assist,

};

public class uAction
{
	public _ACTION eAct { set; get;}
	public int nActIdent { set; get;}
	public int nTarIdent { set; get;}

	public int nTarGridX { set; get;}
	public int nTarGridY { set; get;}

	public int nSkillID { set; get;}

	public int nActVar1 { set; get;}
	public int nActVar2 { set; get;}

};


public partial class ActionManager
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }

//	uAction					CurAction;
//	uAction					LastAction;
//	int 					nActFlow	=0;

	List< uAction > 		ActionPool;				// record all action to do 
//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();

	public void Initial( ){
		hadInit = true;
		//this.GetAudioClipFunc = getAudioClipFunc;
		ActionPool = new List< uAction >();
	//	UnitPool = new Dictionary< int , UNIT_DATA >();
	//	CampPool = new Dictionary< _CAMP , cCamp >();
	}

	private static ActionManager instance;
	public static ActionManager Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new ActionManager();

				instance.Clear();
			}
			
			return instance;
		}
	}

	public void Clear()
	{
		if( ActionPool != null )
			ActionPool.Clear ();
	}

	public bool Run()
	{
	

		if (ActionPool != null && ActionPool.Count > 0) {
			uAction act = ActionPool[0];

			if( act != null )
			{
//		CurAction = act;
			
				Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( act.nActIdent );
				if( unit.SetAction( act ) )
				{
					ActionPool.RemoveAt( 0 ); // remove when setup success
					return true;
				}


			}
			else{
				ActionPool.RemoveAt( 0 );
				Debug.LogError( "Actmanager run null act!!" );
			}
		}
		return false;
	}

//	public void Run()
//	{

//		return ;
//	}


	public uAction CreateAction( int nIdent , _ACTION act )
	{
		uAction p = new uAction ();
		p.eAct = act;
		p.nActIdent = nIdent;
		if( ActionPool == null ){
			ActionPool =  new List< uAction >();
		}
		ActionPool.Add (p); 
		return p;
	}
	//
	public uAction CreateAttackAction( int nAtkIdent , int nDefIdent , int nSkillID , int nVar1=0, int nVar2=0 )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._ATK);
		if( act != null )  {
			act.nTarIdent = nDefIdent;
			act.nSkillID = nSkillID;
			act.nActVar1 = nVar1;
			act.nActVar2 = nVar2;
		}
		return act;
	}
	public uAction CreateMoveAction( int nAtkIdent , int X , int Y , int nVar1=0, int nVar2=0 )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._MOVE);
		if( act != null )  {
			act.nTarGridX = X;
			act.nTarGridY = Y;
			act.nActVar1 = nVar1;
			act.nActVar2 = nVar2;
		}
		return act;
	}

	public uAction CreateWaitingAction( int nAtkIdent  )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._WAIT);
		if( act != null )  {

		}
		return  act;
	}
};

