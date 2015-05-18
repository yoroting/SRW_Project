﻿/* <HexTileManager is an open source tile placing tool kit for Unity3D.>
    Copyright (C) <2014>  <Hexusreaper A.K.A. Soujanya Kumar Barman>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>. 
    
    If you wish to contact me, mail me at "soujanya.barman1992@gmail.com"
 */

using UnityEngine;
using UnityEditor;
using System.Collections;



[CustomEditor(typeof(TileMain))]
public class TileMainEditor : Editor
{
    //Reference to TileMain class
    TileMain tilemain;
    //store mouse hit pos
    private Vector3 mouseHitPos;
    //store actual pixels to unity unit
    private Vector2 tilesize;
    // Tile position based on mouselocation
    private Vector3 tilepos;
    //bool to check if tile selection grid is hidden or not
    public bool isTilesetDone = false;
    // Scrolebar position
    private Vector2 scrolepos = Vector2.zero;
    // String for button
    string tilebutton = "Show Tiles";
    //Checks for Texture2D changes
    public Texture2D checktexture2d;
    //Check variable for all bool values
    public bool checkbool;
    //Check variable for all vector2 values
    public Vector2 checkvector2;
    //Check variable for int values
    public int checkint;
    //starts when scene view is enabled
    public void OnEnable()
    {
        // Reference to the Tilemain Script
        tilemain = (TileMain)target;
        tilesize.x = tilemain.PixelSize.x / tilemain.pixeltounit;
        tilesize.y = tilemain.PixelSize.y / tilemain.pixeltounit;
        //Debug.Log(tilemain.Tiles[0].textureRect.height / tilemain.Tiles[0].bounds.size.y);
        
        
    }
    //All the work happinning in Scene view
    void OnSceneGUI()
    {
        if (tilemain.isdrawmode)
        {
            //Grab the current event
            Event e = Event.current;
            if (e.isMouse)
            {
                // Set the view tool active
                Tools.current = Tool.View;
                //FPS tool is selected
                Tools.viewTool = ViewTool.FPS;
            }
            //Raycast from camera to mouse position 
            Ray r = HandleUtility.GUIPointToWorldRay(new Vector2(e.mousePosition.x, e.mousePosition.y));
            mouseHitPos = r.origin;
            
            // checks if mouse is on the gameobject layer
            if (IsMouseOnLayer())
            {
                //grab the marker position from the mouse position
                tilemain.MarkerPosition = MouseOnTile();
                // refresh the sceneview to update all the chenges 
                if (e.isMouse)
                    SceneView.RepaintAll();
                //checks which mouse button is clicked and dragged
                if (e.type == EventType.MouseDown && e.button == 0 || e.type == EventType.MouseDrag && e.button == 0)
                {
                    if (isTilesetDone)
                        Draw();
                    e.Use();

                }
                if (e.type == EventType.MouseDown && e.button == 1 || e.type == EventType.MouseDrag && e.button == 1)
                {
                    Delete();
                    e.Use();

                }
            }
            //show the gui on scene view
            Handles.BeginGUI();
            GUI.Label(new Rect(10, Screen.height - 90, 100, 100), "LMB: Draw");
            GUI.Label(new Rect(10, Screen.height - 105, 100, 100), "RMB: Erase");
            Handles.EndGUI();
        }
    }
    // overrides the default GUI for the TileMain script 
    public override void OnInspectorGUI()
    {
        //showing properties window
        tilemain.showProperties = EditorGUILayout.Foldout(tilemain.showProperties, "Properties");
        if (tilemain.showProperties)
        {

            checkbool = tilemain.isdrawmode;
            //Create a box layout
            GUILayout.BeginVertical("box");
            tilemain.isdrawmode = GUILayout.Toggle(tilemain.isdrawmode, " Draw Mode");
            if (checkbool != tilemain.isdrawmode)
            {
                //repaints the Sceneview to show the changes
                SceneView.RepaintAll();
                // Set the view tool active
                Tools.current = Tool.View;
                //FPS tool is selected
                Tools.viewTool = ViewTool.FPS;
            }
            GUILayout.BeginHorizontal();
            //SpriteSheet GUI
            GUILayout.Label("SpriteSheet: ");
            checktexture2d = tilemain.SpriteSheet;
            tilemain.SpriteSheet = (Texture2D)EditorGUILayout.ObjectField(tilemain.SpriteSheet, typeof(Texture2D), false);

            GUILayout.EndHorizontal();
            if (checktexture2d != tilemain.SpriteSheet)
            {
                //Generate Tiles
                if (tilemain.SpriteSheet)
                {
                    GenerateTiles();

                }
            }
            GUILayout.BeginHorizontal();
            checkint = tilemain.pixeltounit;
            GUILayout.Label("Pixel To Unit:");
            tilemain.pixeltounit = EditorGUILayout.IntField(tilemain.pixeltounit);
            GUILayout.EndHorizontal();
            if (checkint != tilemain.pixeltounit)
                OnEnable();
            //SizeGui
            checkvector2 = tilemain.PixelSize;
            tilemain.PixelSize = EditorGUILayout.Vector2Field("Pixel Size: ", tilemain.PixelSize);
            if (checkvector2 != tilemain.PixelSize)
            {
                OnEnable();
            }
            tilemain.LayerSize = EditorGUILayout.Vector2Field("Layer Size: ", tilemain.LayerSize);
            tilemain.Level = EditorGUILayout.FloatField("Level :", tilemain.Level);
            // Add Collider GUI
            tilemain.addcollider = GUILayout.Toggle(tilemain.addcollider, " Add Collider (Experimental)");
            if (tilemain.addcollider)
                tilemain.coltyp = EditorGUILayout.Popup(tilemain.coltyp, tilemain.collidertype);
            GUILayout.EndVertical();
        }
           //Tiles GUI
            EditorGUILayout.LabelField("Tiles", EditorStyles.boldLabel);

            //Show/Hide Tiles
                if (GUILayout.Button(tilebutton) && tilemain.SpriteSheet && tilemain.isTileGenerated)
                {

                    if (tilemain.tilesNo > 0)
                    {

                        
                        if (isTilesetDone == false)
                        {
                            isTilesetDone = true;
                            tilebutton = "Hide Tiles";
                        }
                        else
                        {
                            isTilesetDone = false;
                            tilebutton = "Show Tiles";
                        }

                    }
                    else
                    {
                        isTilesetDone = false;
                        Debug.Log("Must select a texture with sprites.");
                    }
                }

            //Show scroll bar For next layout
            scrolepos = GUILayout.BeginScrollView(scrolepos);
            //if tile preview is generated draw a selection grid with all the tiles generated
            if (isTilesetDone)
            {
                tilemain.tileGridId = GUILayout.SelectionGrid(tilemain.tileGridId, tilemain.asset, 6, tilemain.texButton);
             
            }
            GUILayout.EndScrollView();

            
        
        //If the values in the editor is changed
        if (GUI.changed)
        {
            //set the current object as a dirty prefab so it wont lode the default values from the prefab
            EditorUtility.SetDirty(tilemain);
            
        }
    }
    //Generation function for asset texture from an array of sprites
    public void assetPreviewGenerator()
    {
         
        for (int i = 0; i < tilemain.tilesNo; i++)
        {
            
            //Store all the images of sprite in a dynamic array
            if (!tilemain.asset[i])
            {
                //try catch block to catch exception for texture not readable error due to fast execution.
                try
                {
                    //get the tiles rect then create an blank texture thn populate that blank texture with the image of the tile.
                    Rect rect = tilemain.Tiles[i].rect;
                    tilemain.asset[i] = new Texture2D((int)rect.width, (int)rect.height);
                    Color[] pixels = tilemain.SpriteSheet.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
                    tilemain.asset[i].SetPixels(pixels);
                    tilemain.asset[i].Apply();
                }
                //if the exception occur generate the tiles again.
                catch(UnityException e)
                {
                    if(e.Message.StartsWith("Texture '" + tilemain.SpriteSheet.name + "' is not readable"))
                    {
                        GenerateTiles();
                    }
                }
                   
                    
                 
            }
           
            
        }
        
        
    }
    // Drawing function
    private void Draw()
    {
        //Checks if a game object has been already created on that place
        if (!tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)))
        {

            //lets you undo editor changes
            Undo.IncrementCurrentGroup();
            // Instantiate a gameobject with the selected sprite and selected grid location and as a children of main layer 
            GameObject tile = new GameObject("tile");
            SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = tilemain.Tiles[tilemain.tileGridId];
            tile.transform.position = tilemain.MarkerPosition;
            if (tilemain.addcollider)
                UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(tile, "Assets/HexTileManager/Editor/TileMainEditor.cs (272,17)", tilemain.collidertype[tilemain.coltyp]);
            tile.name = string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y,tilepos.z);
            tile.transform.parent = tilemain.transform;
            Undo.RegisterCreatedObjectUndo(tile, "Create Tile");
        }
        // checks if a game object is already located on that location,if true change she sprite of that gameobject
        if (tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)))
        {
            Undo.RecordObject(tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)).GetComponent<SpriteRenderer>().sprite, "Change Sprite");
            tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)).GetComponent<SpriteRenderer>().sprite = tilemain.Tiles[tilemain.tileGridId];
        }
    }
    // Delete Function
    private void Delete()
    {
        if (tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y,tilepos.z)))
        {
            Undo.IncrementCurrentGroup();
            Undo.DestroyObjectImmediate(tilemain.transform.Find(string.Format("Tile_{0}_{1}_{2}", tilepos.x, tilepos.y, tilepos.z)).gameObject);

        }
    }
    // checks if the mouse is on the layer
    private bool IsMouseOnLayer()
    {
        // return true or false depending if the mouse is positioned over the map
        if (mouseHitPos.x > tilemain.transform.position.x && mouseHitPos.x < (tilemain.transform.position.x + (tilemain.LayerSize.x * tilemain.PixelSize.x / tilemain.pixeltounit)) && mouseHitPos.y > tilemain.transform.position.y && mouseHitPos.y < (tilemain.transform.position.y + (tilemain.LayerSize.y * tilemain.PixelSize.y / tilemain.pixeltounit)))
        {
            
            return (true);
        }
        
        return (false);
    }
    //returns the location of the marker based on mouse on grid position
    private Vector3 MouseOnTile()
    {
        //converting the mouse hit position to local coordinates
        Vector2 localmouseHitPos = mouseHitPos - new Vector3(tilemain.transform.position.x, tilemain.transform.position.y, 0);
        // return the column and row values on which the mouse is on
        tilepos = new Vector3((int)(localmouseHitPos.x / tilesize.x), (int)(localmouseHitPos.y / tilesize.y), tilemain.Level);
        //calculate the marker position based on word coordinates
        Vector2 pos = new Vector2(tilepos.x * tilesize.x, tilepos.y * tilesize.y);
        //set the marker position value    

        Vector2 marker = new Vector2(pos.x + tilesize.x / 2, pos.y + tilesize.y / 2) + new Vector2(tilemain.transform.position.x, tilemain.transform.position.y);
       
        return(new Vector3(marker.x,marker.y,(-tilemain.Level)));
                

    }
    // genaration of the tile from tile map
    void GenerateTiles()
    {
        tilemain.isTileGenerated = false;
        //hides the tiles if they are already shown
        isTilesetDone = false;
        tilebutton = "Show Tiles";
        tilemain.Tiles.Clear();
        //location of SpriteSheet
        string path = AssetDatabase.GetAssetPath(tilemain.SpriteSheet);
        TextureImporter A = (TextureImporter)AssetImporter.GetAtPath(path);
        //change the texture isReadable flag to true and reimport the asset
        A.isReadable = true;
        AssetDatabase.ImportAsset(path);
        //dyanamic array to store the sprites and fill it with sprites
        object[] objs;
        objs = AssetDatabase.LoadAllAssetsAtPath(path);
       
        tilemain.tilesNo = objs.Length - 1;
        tilemain.asset = new Texture2D[tilemain.tilesNo];
        if (tilemain.tilesNo > 0)
        {
            //Storing Tiles as Sprites
            for (int i = 1; i <= objs.Length - 1; i++)
            {
                tilemain.Tiles.Add((Sprite)objs[i]);
                
            }
            //generate the tiles preview
            assetPreviewGenerator();
            tilemain.isTileGenerated = true;
            Debug.Log("New Tiles Generated");
        }
        else
            Debug.Log("Must select a texture with sprites.");
    }

    
}
