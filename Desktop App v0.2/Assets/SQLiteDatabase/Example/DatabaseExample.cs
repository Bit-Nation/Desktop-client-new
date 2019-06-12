using UnityEngine;
using System.Collections;
using SQLiteDatabase;
using System.Collections.Generic;

public class DatabaseExample : MonoBehaviour {

	SQLiteDB db = SQLiteDB.Instance;

	List<string> allIDs = new List<string>();
	List<string> allNames = new List<string>();

	// Events
	void OnEnable()
	{
		SQLiteEventListener.onError += OnError;
	}

	void OnDisable()
	{
		SQLiteEventListener.onError -= OnError;
	}

	void OnError(string err)
	{
		Debug.Log (err);
	}

	// Use this for initialization
	void Start () {
		// set database location (directory)
		db.DBLocation = Application.persistentDataPath;
		db.DBName = "MyDatabase.db";
		Debug.Log ("Database Directory : "+db.DBLocation);

		if (db.Exists) {
			ConnectToDB ();
		}
	}


	// UI
	string _id = "";
	string _name = "";
	Vector2 scrollPos = Vector2.zero;
	string btnName_SaveUpdate = "Save";
	string btnName_CreateDeleteTable = "Create Table";
	bool isTableCreated = false;

	void OnGUI()
	{
		Rect rect = new Rect (0, 0, 100, 20);
		// Create database first
		if (!db.Exists) {
			if(GUI.Button(new Rect(Screen.width/2-125,Screen.height/2-40,250,80),"Create DB"))
			{
				CreateDB();
			}
			return;
		}

		// after creating database and a table in DB
		if (isTableCreated) {
			GUI.Label (rect, "ID");
			rect.x = 150;
			GUI.Label (rect, "Name");

			rect.y = 30;
			rect.x = 0;
			if (btnName_SaveUpdate == "Save")
				_id = GUI.TextField (rect, _id);
			else
				GUI.TextField (rect, _id);

			rect.x = 150;
			_name = GUI.TextField (rect, _name);
			
			rect.x = rect.x + rect.width + 10;
			if (GUI.Button (rect,btnName_SaveUpdate)) {
				if(btnName_SaveUpdate == "Save")
				{
					AddRow(_id,_name);
				}
				else if(btnName_SaveUpdate == "Update")
				{
					UpdateRow(_id,_name);
				}
			}
		}


		// Display all data from database
		rect.x = 0;
		rect.y = 60;
		rect.width = Screen.width;
		rect.height = Screen.height - 120;

		scrollPos = GUI.BeginScrollView (rect, scrollPos, new Rect (0,0,rect.width-30,allIDs.Count*30));
		rect.width = 125;
		rect.height = 25;
		for (int i=0; i<allIDs.Count; i++) {
			// id label
			rect.x = 0;
			rect.y = i * 25;
			GUI.Label(rect,allIDs[i]);
			// name
			rect.x += 130;
			GUI.Label(rect,allNames[i]);
			// update button
			rect.x += 130;
			if(GUI.Button(rect,"Edit"))
			{
				_id = allIDs[i];
				_name = allNames[i];
				btnName_SaveUpdate = "Update";
			}
			// remove button
			rect.x += 130;
			if(GUI.Button(rect,"Delete"))
			{
				DeleteRow(allIDs[i]);
			}
		}
		GUI.EndScrollView ();

		// clear table
		rect.y = Screen.height - 25;
		rect.x = 0;
		rect.width = 100;
		rect.height = 20;
		GUI.enabled = isTableCreated;
		if (GUI.Button (rect, "Clear Table")) {
			db.ClearTable("Users");

			Refresh();
		}
		GUI.enabled = true;

		// delete table
		rect.x += 110;
		if (GUI.Button (rect, btnName_CreateDeleteTable)) {
			if(btnName_CreateDeleteTable == "Delete Table" && db.DeleteTable("Users"))
			{
				allIDs.Clear();
				allNames.Clear();
				btnName_CreateDeleteTable = "Create Table";
				isTableCreated = false;
			}
			else if(btnName_CreateDeleteTable == "Create Table")
			{
				if(CreateTable())
				{
					btnName_CreateDeleteTable = "Delete Table";
					isTableCreated = true;
				}
			}
		}
		
		// delete table
		rect.x += 110;
		if (GUI.Button (rect, "Delete DB")) {

			if(db.DeleteDatabase())
			{
				allIDs.Clear();
				allNames.Clear();
				btnName_CreateDeleteTable = "Create Table";
				isTableCreated = false;
			}
		}
	}

	// create database and table
	void CreateDB()
	{
		// create a fresh database at specified location
		//	db.CreateDatabase (db.DBName,true);

		// use default database from StreamingAssets folder
		db.ConnectToDefaultDatabase (db.DBName,true);

		Refresh ();
	}

	void ConnectToDB()
	{
		//db.CreateDatabase (db.DBName,false);

		db.ConnectToDefaultDatabase (db.DBName,false);
		Refresh ();
	}

	bool CreateTable()
	{
		// database created successfuly then create table
		if (db.Exists) {
			// Create table :: schema
			// you can create table when you want
			DBSchema schema = new DBSchema ("Users");
			schema.AddField ("Id", SQLiteDB.DB_DataType.DB_INT, 0, false, true, true);
			schema.AddField ("Name", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
			return db.CreateTable (schema);
		}

		return false;
	}

	// add a single entry in database
	void AddRow(string id, string name)
	{
		List<SQLiteDB.DB_DataPair> dataPairList = new List<SQLiteDB.DB_DataPair>();
		SQLiteDB.DB_DataPair data = new SQLiteDB.DB_DataPair();
		// Insert first row
		// first field
		data.fieldName = "Id";
		data.value = id;
		dataPairList.Add(data);
		
		// second field
		data.fieldName = "Name";
		data.value = name;
		dataPairList.Add(data);
		
		// insert into Users - first row
		int i = db.Insert("Users",dataPairList);

		if (i > 0)
		{
			Debug.Log("Record Inserted!");
			_name = "";
			_id = "";
			Refresh ();
		}

	}

	// update a row
	void UpdateRow(string id, string name)
	{
		// list of data to be updated
		List<SQLiteDB.DB_DataPair> dataList = new List<SQLiteDB.DB_DataPair> ();

		// data to be updated
		SQLiteDB.DB_DataPair data = new SQLiteDB.DB_DataPair ();
		data.fieldName = "Name";
		data.value = name;

		dataList.Add (data);

		// row to be updated
		SQLiteDB.DB_ConditionPair condition = new SQLiteDB.DB_ConditionPair ();
		condition.fieldName = "Id";
		condition.value = id;
		condition.condition = SQLiteDB.DB_Condition.EQUAL_TO;

		int i = db.Update ("Users", dataList, condition);

		if (i > 0)
		{
			Debug.Log(i + " Record Updated!");
			_name = "";
			_id = "";
			Refresh ();
		}
	}

	// delete perticular id
	void DeleteRow(string id)
	{
		SQLiteDB.DB_ConditionPair condition = new SQLiteDB.DB_ConditionPair ();
		// delete from Users where Id = id
		condition.fieldName = "Id";
		condition.value = id;
		condition.condition = SQLiteDB.DB_Condition.EQUAL_TO;

		int i = db.DeleteRow ("Users", condition);
		if (i > 0)
		{
			Debug.Log(i + " Record Deleted!");
			Refresh ();
		}
	}

	void Refresh()
	{
		allIDs.Clear ();
		allNames.Clear ();

		// get all data from Users table
		DBReader reader = db.GetAllData("Users");

		while(reader != null && reader.Read())
		{
			allIDs.Add(reader.GetStringValue("Id"));
			allNames.Add(reader.GetStringValue("Name"));
			isTableCreated = true;
		}

		btnName_SaveUpdate = "Save";
	}

	// use this to avoid any lock on database, otherwise restart editor or application after each run
	void OnApplicationQuit()
	{
		// release all resource using by database.
		db.Dispose ();
	}
}
