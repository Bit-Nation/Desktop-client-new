using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;
using SQLiteDatabase;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;  
using System.Xml; 
using System.Xml.Serialization; 

public class Main : MonoBehaviour
{
    
	// app version
	private string app_version = "1.0";
	
	// production enviroment : 1 = true / 0 = false (test enviroment)
	private int production = 0;
	
	// Variables to store Screen Size (used to control the resize of the screen window)
	private float screen_width;
	private float screen_height;
	
	
	// Main Database
	SQLiteDB db = SQLiteDB.Instance;
	
	// Main Variables
	private string token;
	private string id_user;
	private string last_update;
	private string current_nation;
	
	// PREFABS	
	public Transform menu_nation;
	public Transform new_nation;
	public Transform menu_item;
	
	public Transform paragraph;
	public Transform button;

	// Start is called before the first frame update
    void Start()
    {
        
		log("== INIT config ==\r\n");
		
			GameObject.Find("Screens").transform.Find("Console").gameObject.SetActive(false);
			log("    > Console set to inactive");

			screen_width = Screen.width;
			log("    > screen_width set to " + screen_width);
			
			screen_height = Screen.height;
			log("    > screen_height set to " + screen_height);
			
			GameObject.Find("Screens").transform.Find("Sidebar/Menu").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-142));
			log("    > Sidebar/Menu size changed to : "+GameObject.Find("Screens").transform.Find("Sidebar/Menu").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Sidebar/Nations").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-142));
			log("    > Sidebar/Nations size changed to : "+GameObject.Find("Screens").transform.Find("Sidebar/Nations").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Sidebar/Contacts").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-142));
			log("    > Sidebar/Contacts size changed to : "+GameObject.Find("Screens").transform.Find("Sidebar/Contacts").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Sidebar/Settings").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-142));
			log("    > Sidebar/Settings size changed to : "+GameObject.Find("Screens").transform.Find("Sidebar/Settings").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Page").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-272.5), (float)(Screen.height));
			log("    > Page size changed to : "+GameObject.Find("Screens").transform.Find("Page").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Page/Content").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-50));
			log("    > Page size changed to : "+GameObject.Find("Screens").transform.Find("Page/Content").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Page").gameObject.SetActive(false);
			log("    > Page set to inactive");

			// Application.targetFrameRate = 60;
			// log("    > FrameRate set to " + Application.targetFrameRate);
			
			// QualitySettings.vSyncCount = 0;
			// log("    > vSyncCount set to " + QualitySettings.vSyncCount);
	
			
			// MAIN DATABASE
			
			db.DBName = "Main.db";
			db.DBLocation = Application.persistentDataPath;
			bool result = db.CreateDatabase(db.DBName, false); // false = do not overwrite if it exists already
			
			if(result == true)
			{
				
				log("    > Main DB CREATED at: " + Application.persistentDataPath);	
				
				create_tables();
				
			}
			else
			{
			
				log("    > Main DB creation error: " + result);		
							
			}
			
		log("\r\n== END config ==\r\n");
		
			// Call to setup all Screen Functions (hide all and show login screen
		
			message("hide", "");
		
			lobby("Login");
			
			Sidebar("hide");
						
    }

    // Update is called once per frame
    void Update()
    {
        
		if(screen_height != Screen.height || screen_width != Screen.width)
		{
		
			log("\r\n== INIT screen update ==\r\n");
			
			GameObject.Find("Screens").transform.Find("Sidebar/Menu").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-142));
			log("    > Sidebar/Menu size changed to : "+GameObject.Find("Screens").transform.Find("Sidebar/Menu").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Sidebar/Nations").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-142));
			log("    > Sidebar/Nations size changed to : "+GameObject.Find("Screens").transform.Find("Sidebar/Nations").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Sidebar/Contacts").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-142));
			log("    > Sidebar/Contacts size changed to : "+GameObject.Find("Screens").transform.Find("Sidebar/Contacts").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Sidebar/Settings").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-142));
			log("    > Sidebar/Settings size changed to : "+GameObject.Find("Screens").transform.Find("Sidebar/Settings").GetComponent<RectTransform>().sizeDelta);
						
			GameObject.Find("Screens").transform.Find("Page").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-272.5), (float)(Screen.height));
			log("    > Page size changed to : "+GameObject.Find("Screens").transform.Find("Page").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens").transform.Find("Page/Content").GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(Screen.height-50));
			log("    > Page size changed to : "+GameObject.Find("Screens").transform.Find("Page/Content").GetComponent<RectTransform>().sizeDelta);
			
			screen_height = Screen.height;
			screen_width = Screen.width;
			
			log("\r\n== END screen update ==\r\n");			
		
		}
		
		// detect F1 key pressed for console
		if (Input.GetKeyDown(KeyCode.F1))
        {
			
			log("\r\n== F1 key pressed ==\r\n");
			
				if(GameObject.Find("Console")){
					
					GameObject.Find("Screens").transform.Find("Console").gameObject.SetActive(false);
					log("    > Console set to inactive");
									
				}else{
					
					GameObject.Find("Screens").transform.Find("Console").gameObject.SetActive(true);
					log("    > Console set to active");					
					
				}
			
			log("\r\n== END F1 key ==\r\n");			
			
        }		
		
		
		
    }
	
	// ***********************************
	// 		Interactive Functions
	// ***********************************
	
	// Click on the login button
	public void login()
	{
		
		message("loading", "Connecting to server ...");
		
		string json = "{\"request\": \"login\",\"username\": \""+get_value("Input_username")+"\",\"password\": \""+get_value("Input_password")+"\"} ";		
			
		StartCoroutine(apicom(json));
			
	}
	
	// Click on the login button
	public void new_user()
	{
		
		if(get_value("Input_new_password") != get_value("Input_new_repassword")){
			
			message("error", "Retyped password mismatch.");
		
			return;
			
		}
		
		
		message("loading", "Connecting to server ...");
		
		string json = "{\"request\": \"new_user\","+
		
			"\"username\": \""+get_value("Input_new_username")+"\","+
			
			"\"citzen_id\": \""+get_value("Input_new_id")+"\","+
			
			"\"name\": \""+get_value("Input_new_name")+"\","+
			
			"\"password\": \""+get_value("Input_new_password")+"\","+
			
			"\"email\": \""+get_value("Input_new_email")+"\""+
						
		"} ";		
		
		StartCoroutine(apicom(json));
			
	}
		
	// Change the text from the create new user screen
	public void change_id()
	{
		
		set_text("Text_id", get_value("Input_new_id")+"@bitnation");
		
	}
					
	// Click on the login button
	public void retrieve()
	{
		
		message("loading", "Connecting to server ...");
		
		string json = "{ \"request\": \"retrieve\",\"email\": \""+get_value("Input_email")+"\" } ";		
			
		StartCoroutine(apicom(json));
			
	}		

	// Click on the join new nation button
	public void join_nation()
	{
		
		string json = "{ \"request\": \"join_nation\",\"token\": \""+token+"\",\"nation_id\": \""+get_value("Input_nation_id")+"\",\"nation_name\": \""+get_value("Input_nation_name")+"\" } ";		
				
		StartCoroutine(apicom(json));

		message("loading", "Joining nation ...");
		
		
		
	}		



	// Click okay on error message screen
	public void click_okay()
	{
				
		message("hide", "");
					
	}

	// Logs out and resets the app
	public void logout()
	{
		
		SceneManager.LoadScene( SceneManager.GetActiveScene().name );
				
	}
		
	// ***********************************
	// 		Screen Functions
	// ***********************************
		
	// Shows messages on top of all other screens
	private void message(string screen, string message)
	{
		
		log("\r\n    == INIT message ==\r\n");
		
			if(screen == "hide")
			{
				
				GameObject.Find("Screens").transform.Find("Message").gameObject.SetActive(false);
				log("        > Message set to inactive");
								
			}
			else
			{
				
				// Activate the main message screen with black background
				
				GameObject.Find("Screens").transform.Find("Message").gameObject.SetActive(true);
				log("        > Message set to active");
									
				// Sets all sub screen to inactive
				
				GameObject.Find("Screens/Message").transform.Find("Loading").gameObject.SetActive(false);
				log("        > Message/Loading set to inactive");	

				GameObject.Find("Screens/Message").transform.Find("Error").gameObject.SetActive(false);
				log("        > Message/Error set to inactive");						
				
				GameObject.Find("Screens/Message").transform.Find("Version").gameObject.SetActive(false);
				log("        > Message/Version set to inactive");	
				
				GameObject.Find("Screens/Message").transform.Find("New_nation").gameObject.SetActive(false);
				log("        > Message/New_nation set to inactive");						
				
				if(screen == "loading")
				{
					
					GameObject.Find("Screens/Message").transform.Find("Loading").gameObject.SetActive(true);
					log("        > Message/Loading set to active");		
					
					set_text("Screens/Message/Loading/Mask/Text", message);
															
				}
				else if(screen == "error")
				{
					
					GameObject.Find("Screens/Message").transform.Find("Error").gameObject.SetActive(true);
					log("        > Message/Error set to active");		
					
					GameObject.Find("Screens/Message/Error/Mask/Text").GetComponent<TextMeshProUGUI>().text = message;
										
				}
				else if(screen == "version")
				{
					
					GameObject.Find("Screens/Message").transform.Find("Version").gameObject.SetActive(true);
					log("        > Message/Version set to active");		
										
				}
				else if(screen == "new_nation")
				{
					
					GameObject.Find("Screens/Message").transform.Find("New_nation").gameObject.SetActive(true);
					log("        > Message/New_nation set to active");		
						
					focus("Input_nation_id");
						
				}
								
			}
					
		log("\r\n    == END message ==\r\n");
		
	}
	
	// Shows one screen from the lobby (or hide all of them)
	// This function is called either inside this code or directly from buttons (onclick method)
	public void lobby(string screen)
	{

		log("== INIT Lobby ==\r\n");
									
				
				GameObject.Find("Screens").transform.Find("Background").gameObject.SetActive(true);
				log("    > Background set to Active");
				
				GameObject.Find("Screens").transform.Find("Login").gameObject.SetActive(false);
				log("    > Login set to inactive");
				
				GameObject.Find("Screens").transform.Find("Create_account").gameObject.SetActive(false);
				log("    > Create_account set to inactive");

				GameObject.Find("Screens").transform.Find("Restore_account").gameObject.SetActive(false);
				log("    > Restore_account set to inactive");
				
				// This shows the proper screen and sets focus to the proper input field
				// If you pass Hide or any other parameter then it will not show any screen from the lobby
						
				if(screen == "hide")
				{
					
					GameObject.Find("Screens").transform.Find("Background").gameObject.SetActive(false);
					log("    > Background set to inactive");
					
				}
				else if(screen == "Login")
				{
					
					GameObject.Find("Screens").transform.Find("Login").gameObject.SetActive(true);
					log("    > Login set to active");
					
					focus("Input_username");
					log("    > Focus set to Input_username");
					
				}
				else if(screen == "Create")
				{
					
					GameObject.Find("Screens").transform.Find("Create_account").gameObject.SetActive(true);
					log("    > Create_account set to active");
					
					focus("Input_new_username"); // Input_new_name1
					log("    > Focus set to Input_new_username");
										
				}
				else if(screen == "Restore")
				{
					
					GameObject.Find("Screens").transform.Find("Restore_account").gameObject.SetActive(true);
					log("    > Restore_account set to active");
					
					focus("Input_email");
					log("    > Focus set to Input_email");
					
					
				}
				
			
		log("\r\n== END Lobby ==\r\n");
		
	}

	// Shows the sidebar and its tabs / or hide it
	// This function is called either inside this code or directly from buttons (onclick method)
	public void Sidebar(string tab)
	{

		log("== INIT Sidebar ==\r\n");
										
			if(tab == "hide"){
				
				
				GameObject.Find("Screens").transform.Find("Sidebar").gameObject.SetActive(false);
				log("    > Screens/Sidebar set to inactive");
			
			}
			else
			{
				
				GameObject.Find("Screens").transform.Find("Sidebar").gameObject.SetActive(true);
				log("    > Screens/Sidebar set to inactive");

				GameObject.Find("Screens/Sidebar/Tab1").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
				log("    > Tab1 color set to : "+GameObject.Find("Screens/Sidebar/Tab1").GetComponent<Image>().color);
			
				GameObject.Find("Screens/Sidebar/Tab2").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
				log("    > Tab1 color set to : "+GameObject.Find("Screens/Sidebar/Tab2").GetComponent<Image>().color);
			
				GameObject.Find("Screens/Sidebar/Tab3").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
				log("    > Tab1 color set to : "+GameObject.Find("Screens/Sidebar/Tab3").GetComponent<Image>().color);
			
				GameObject.Find("Screens/Sidebar/Tab4").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
				log("    > Tab1 color set to : "+GameObject.Find("Screens/Sidebar/Tab4").GetComponent<Image>().color);
			
				GameObject.Find("Screens/Sidebar").transform.Find("Menu").gameObject.SetActive(false);
				log("    > Sidebar/menu set to inactive");
				
				GameObject.Find("Screens/Sidebar").transform.Find("Contacts").gameObject.SetActive(false);
				log("    > Sidebar/Contacts set to inactive");
			
				GameObject.Find("Screens/Sidebar").transform.Find("Nations").gameObject.SetActive(false);
				log("    > Sidebar/Nations set to inactive");
			
				GameObject.Find("Screens/Sidebar").transform.Find("Settings").gameObject.SetActive(false);
				log("    > Sidebar/Settings set to inactive");
			
				if(tab == "Tab1"){
								
					GameObject.Find("Screens/Sidebar/Tab1").GetComponent<Image>().color = new Color(0.5490196f, 0.5803922f, 0.6666667f, 1f);
					log("    > Tab1 color set to : "+GameObject.Find("Screens/Sidebar/Tab1").GetComponent<Image>().color);
			
					GameObject.Find("Screens/Sidebar").transform.Find("Menu").gameObject.SetActive(true);
					log("    > Sidebar/menu set to inactive");
				
				}
				else if(tab == "Tab2")
				{
					
					GameObject.Find("Screens/Sidebar/Tab2").GetComponent<Image>().color = new Color(0.5490196f, 0.5803922f, 0.6666667f, 1f);
					log("    > Tab2 color set to : "+GameObject.Find("Screens/Sidebar/Tab2").GetComponent<Image>().color);
					
					GameObject.Find("Screens/Sidebar").transform.Find("Nations").gameObject.SetActive(true);
					log("    > Sidebar/Nations set to inactive");
					
					
				}
				else if(tab == "Tab3")
				{
					
					GameObject.Find("Screens/Sidebar/Tab3").GetComponent<Image>().color = new Color(0.5490196f, 0.5803922f, 0.6666667f, 1f);
					log("    > Tab3 color set to : "+GameObject.Find("Screens/Sidebar/Tab3").GetComponent<Image>().color);
					
					GameObject.Find("Screens/Sidebar").transform.Find("Contacts").gameObject.SetActive(true);
					log("    > Sidebar/Contacts set to inactive");
				
				}		
				else if(tab == "Tab4")
				{
					
					GameObject.Find("Screens/Sidebar/Tab4").GetComponent<Image>().color = new Color(0.5490196f, 0.5803922f, 0.6666667f, 1f);
					log("    > Tab4 color set to : "+GameObject.Find("Screens/Sidebar/Tab4").GetComponent<Image>().color);
					
					GameObject.Find("Screens/Sidebar").transform.Find("Settings").gameObject.SetActive(true);
					log("    > Sidebar/Settings set to inactive");
						
				}
				
			}
			
		
			
		log("\r\n== END Sidebar ==\r\n");
		
	}

	// Load Menu itens based on the nation (Retrieved from local db)
	public void Load_menu(string nation)
	{
		
		Transform obj;
		Texture2D texture;
		DBReader reader;
			
		clear("Screens/Sidebar/Menu/Viewport/Content");
		
		current_nation = nation;
		
		if(nation == ""){
			
			reader = db.Select("Select * from users_nations WHERE id_user ='" + id_user + "' LIMIT 1");
			
			if(reader != null && reader.Read()){
				
				nation = reader.GetStringValue("id_nation");
				
				log("        > NATION SET TO: " + nation );
									
			}
									
		}	

		reader = db.Select("Select * from users_nations WHERE id_user ='" + id_user + "' AND id_nation = '"+nation+"' LIMIT 1");
		
		// Nation banner on the MENU
		
		if(reader != null && reader.Read()){
			
			obj = Instantiate(menu_nation, new Vector3(0, 0, 0), Quaternion.identity);
				
			obj.SetParent(GameObject.Find("Screens/Sidebar/Menu/Viewport/Content").gameObject.transform, false);

			obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = reader.GetStringValue("name");
		
			obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(80));
			
			string nation_banner = nation;
			
			obj.transform.GetComponent<Button>().onClick.AddListener( 
				
					delegate{ 
					
								request_page("0","",nation_banner); 
								log("        > Clicked on nation banner id: " + nation_banner );
								
							}
							
				);
				
		
			log("        > NATION banner Created");
								
		}else{
			
			log("        > *************** NATION not found with id : "+nation);
					
		}
		
		reader = db.Select("Select * from nations_structure WHERE id_user ='" + id_user + "' AND id_nation ='"+nation+"' AND status='1' ORDER BY menu_order ASC");
		
		log("        > ID USER: "+id_user +" - Nation:"+nation);
			
		
		while (reader != null && reader.Read())
		{
		
			log("        > MENU LOADED: " + reader.GetStringValue("menu_name") + " - " + reader.GetStringValue("menu_icon") );
			
			if(reader.GetStringValue("menu_type") == "1"){
				
				obj = Instantiate(menu_item, new Vector3(0, 0, 0), Quaternion.identity);
				
				string menu_type_id = reader.GetStringValue("menu_type_id");
				string nation_id = reader.GetStringValue("id_nation");
				
				
				obj.transform.GetComponent<Button>().onClick.AddListener( 
				
					delegate{ 
					
								request_page("2",menu_type_id,nation_id); 
								log("        > Clicked on main id: " + menu_type_id );
								
							}
							
				);
				
			
			}
			else if(reader.GetStringValue("menu_type") == "2")
			{
		
				obj = Instantiate(menu_item, new Vector3(0, 0, 0), Quaternion.identity);
				
				string menu_type_id = reader.GetStringValue("menu_type_id");
				string menu_type = reader.GetStringValue("menu_type");
				string nation_id = reader.GetStringValue("id_nation");
				
				
				obj.transform.GetComponent<Button>().onClick.AddListener( 
				
					delegate{ 
					
								request_page("3",menu_type_id,nation_id); 
								log("        > Clicked on menu type id: " + menu_type_id );
								
							}
							
				);
				
			
			}
			else if(reader.GetStringValue("menu_type") == "3")
			{
				
				obj = Instantiate(menu_item, new Vector3(0, 0, 0), Quaternion.identity);
				
				string menu_type_id = reader.GetStringValue("menu_type_id");
				string menu_type = reader.GetStringValue("menu_type");
				string nation_id = reader.GetStringValue("id_nation");
				
				obj.transform.GetComponent<Button>().onClick.AddListener( 
				
					delegate{ 
					
								// request_chat(menu_type_id); 
								log("        > Clicked on chat id: " + menu_type_id );
								
							}
							
				);
							
			}
			else
			{
				
				obj = Instantiate(menu_item, new Vector3(0, 0, 0), Quaternion.identity);
								
			}
					
			
			obj.SetParent(GameObject.Find("Screens/Sidebar/Menu/Viewport/Content").gameObject.transform, false);

			obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = reader.GetStringValue("menu_name");
			
			texture = Resources.Load<Texture2D>("images/"+reader.GetStringValue("menu_icon")); // +reader.GetStringValue("menu_icon")
			
			obj.transform.Find("Icon").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f,0.5f));
			
		};
		
		log("        > Finished Loading Menu \r\n ");
								
		
		// Request the main nation Page
		request_page("0","0",nation);
		
	}

	// Load the contents of a given page
	public void Load_page(string name)
	{
			
		log("== INIT loadpage ==\r\n");
			
			GameObject.Find("Screens").transform.Find("Page").gameObject.SetActive(true);
			log("    > Screens/Page set to active");
		
			GameObject.Find("Screens/Page").transform.Find("Loading").gameObject.SetActive(false);
			log("    > Page/Loading set to inactive");
			
			log("    > Page name: " + name);

			XmlDocument xmlDoc = new XmlDocument(); 
			
			try
			{
				
				xmlDoc.Load( Application.persistentDataPath + "/"+name+".txt" ); 
						
			}
			catch (Exception e)
			{
				
				log("\r\n**********\r\n ERROR : File not found. ("+name+")\r\n**********\r\n");
			
				return;
			
			}
			
			
			// variables used in the page load
		
		Transform obj;
		Texture2D texture;
		string first_field = ""; // used to give focus to the first input field created
		
		XmlNodeList pagelist = xmlDoc.GetElementsByTagName("page");
		
		foreach (XmlNode pageinfo in pagelist) {
			
			if(pageinfo.Attributes["title"] != null){
			
				set_text("Screens/Page/Title/Text", pageinfo.Attributes["title"].Value);
				
				log("    > Page title set to: "+get_text("Screens/Page/Title/Text")); 

				
			}else{
				
				set_text("Screens/Page/Title/Text", "#error");
				
				error("WARNING : Page title not found. (page:"+name+")");
		
			}
			
			clear("Screens/Page/Content/Viewport/Content");
						
			XmlNodeList pagecontent = pageinfo.ChildNodes;
			
			Transform parent = GameObject.Find("Screens/Page/Content/Viewport/Content").gameObject.transform;
			
			foreach (XmlNode pageitems in pagecontent) {
								
				// log(" PAGE :::::: Child name: "+pageitems.Name+" - id : "+pageitems.Attributes["id"].Value+" ");
				
				
				
				if(pageitems.Name == "text")
				{
					
					obj = Instantiate(paragraph, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(parent, false);

					// change thte text

					obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = pageitems.InnerText; // .Replace("{l}", "<").Replace("{b}", ">");
										
					// Change the name of the text for reference
					
					if(pageitems.Attributes["id"] != null)
					{
						
						obj.name = "page_" + pageitems.Attributes["id"].Value;
											
					}else
					{
						
						obj.name = "page_text";
												
					}
					
					log("    > Page text created"); 

					
				}
				else if(pageitems.Name == "button")
				{
					
					obj = Instantiate(button, new Vector3(0, 0, 0), Quaternion.identity);
					
					obj.SetParent(parent, false);

					if(pageitems.Attributes["link_page"] != null)
					{
						
						obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener( delegate { 
													
								// loadpage(pageitems.Attributes["link_page"].Value); 
									
								log("    > ********* Clicked to load page: "+pageitems.Attributes["link_page"].Value); 
	
							});
					
					}
					
					if(pageitems.Attributes["label"] != null)
					{
						
						obj.transform.Find("Button/Label").GetComponent<TextMeshProUGUI>().text = pageitems.Attributes["label"].Value;
											
					}else
					{
						
						obj.transform.Find("Button/Label").GetComponent<TextMeshProUGUI>().text = "#error";
																		
					}
					
					log("    > Page button created"); 
					
				}
				
				
				
				
			}

			
			
			
		}
		
		log("\r\n== END loadpage ==\r\n");
			
		
	
		
	}

	// REquest a page from the API
	public void request_page(string type,string page_id, string nation_id)
	{
		
		GameObject.Find("Screens").transform.Find("Page").gameObject.SetActive(true);
		log("    > Screens/Page set to active");
	
		GameObject.Find("Screens/Page").transform.Find("Loading").gameObject.SetActive(true);
		log("    > Page/Loading set to active");
	
		string json = "{\"request\": \"page\",\"token\": \""+token+"\",\"type\": \""+type+"\",\"page\": \""+page_id+"\",\"nation_id\": \""+nation_id+"\"} ";		
			
		StartCoroutine(apicom(json));
				
	}
	
	// Load the list of nations the user belongs to
	private void Load_nations()
	{
		
		log("\r\n    == INIT load_nations ==\r\n");
				
		Transform obj;
		Texture2D texture;
		DBReader reader;
		
		
		clear("Screens/Sidebar/Nations/Viewport/Content");
		
		// Join new nation
		
			obj = Instantiate(new_nation, new Vector3(0, 0, 0), Quaternion.identity);
			
			obj.name = "new_nation";
			
			obj.SetParent(GameObject.Find("Screens/Sidebar/Nations/Viewport/Content").gameObject.transform, false);

			obj.transform.GetComponent<Button>().onClick.AddListener( 
			
				delegate{ 
				
							message("new_nation","");
							log("        > Clicked to join new nation." );
							
						}
						
			);
		
		// Nation List
		
		reader = db.Select("Select * from users_nations WHERE id_user ='" + id_user + "' ");
		
		while (reader != null && reader.Read())
		{
		
			log("        > Nation loaded: " + reader.GetStringValue("name")+" - ID: "+reader.GetStringValue("id") );
			
			string id_nation = "";
					
			obj = Instantiate(menu_nation, new Vector3(0, 0, 0), Quaternion.identity);
			
			obj.name = reader.GetStringValue("id");
			
			id_nation = reader.GetStringValue("id_nation");
			
			obj.SetParent(GameObject.Find("Screens/Sidebar/Nations/Viewport/Content").gameObject.transform, false);

			obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = reader.GetStringValue("name");
			
			obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (float)(60));
		
			obj.transform.GetComponent<Button>().onClick.AddListener( 
			
				delegate{ 
				
							Load_menu(id_nation); 
							Sidebar("Tab1");
							log("        > Nation changed to id: " + id_nation );
							
						}
						
			);
			
						
		};
		
		
		log("\r\n    == END update_response ==\r\n");
					
	}
	
	// ***********************************
	// 		Communication Functions
	// ***********************************
	
	// Api com function
	IEnumerator apicom(string json)
    {
		
		log("== INIT apicom ==\r\n");
				
		string api_url = "";
		
		// Sets the api URl according to the production enviroment
		
		if(production == 1)
		{
			
			api_url = "http://bitnationapi.azurewebsites.net/api.php";
			
		}
		else if(production == 0)
		{
			
			api_url = "http://bitnationapi.azurewebsites.net/api.php";
			
		}
		else
		{
			
			error("ERROR [#apicom001] : Invalid production enviroment : " + production);
			
			message("error", "Invalid production enviroment : " + production);
		
		}
		
		// Put is used to send raw data. (Json string in our case)
		// The other possible method is POST as variables in a form.
		
		WWWForm form = new WWWForm();
		
        form.AddField("json", json);
		form.AddField("app_version", app_version);
		
        using (UnityWebRequest www = UnityWebRequest.Post(api_url, form))
        {
           	   
			yield return www.SendWebRequest(); // Send();
			
			log("    > API request sent.");
			log("    > JSON:"+json);
						
			log("\r\n== END apicom ==\r\n");
					
			if (www.isNetworkError || www.isHttpError)
            {
				
				error("ERROR [#apicom002] : Network error : " + www.error);
			
				message("error", "Network error : " + www.error);
		
            }
			else
			{
				
				log("== INIT apicom response ==\r\n");
							
				// Show results as text
                log("    > Apicom response : " + www.downloadHandler.text);
			
				// r stores the response from the api call inside an array
                var r = JSON.Parse(www.downloadHandler.text);
				
				if (r == null)
                {

					// Returns null on empty json or with a bad format
					error("ERROR [#apicom003] : Unknown response error.");
			
					message("error", "Unknown response error.");
				
                }
				else
				{
					
					if (r["error"] == "true")
                    {
					
						message("error", r["error_message"]);											
						
					}
					else if (r["error"] == "version")
                    {
						message("version", r["error_message"]);											
						
					}
					else if (r["error"] == "relogin")
                    {
						message("error", r["error_message"]);
																	
					}
					else
					{
						
						if(r["request"] == "login")
						{
							
							login_response(r);
							
						}
						else if(r["request"] == "new_user")
						{
							
							lobby("Login");
							message("error", "New user sucessfully created!");
													
						}
						else if(r["request"] == "update")
						{
							
							update_response(r);
													
						}
						else if(r["request"] == "page")
						{
							
							savefile("temp", r["content"]);
							
							Load_page("temp");
													
						}
						else if(r["request"] == "join_nation")
						{
							
							join_nation_response(r);
													
						}
						else
						{
							
							message("error", "Unknown response request: "+r["request"]);
							
							log("    > ERROR [#apicom003] : Unknown response request: "+r["request"]);
						
						}
						
						
						
						
						/*
						
						if(r["request"] == "login")
						{
							
							login_response(r);
							
						}
						else if(r["request"] == "new_user")
						{
							
							lobby("Login");
							message("error", "New user sucessfully created!");
													
						}
						else if(r["request"] == "retrieve")
						{
							
							message("error", "Check your email with the information.");
													
						}
						else if(r["request"] == "update")
						{
							
							update_response(r);
													
						}
						else if(r["request"] == "page")
						{
							
							request_page_response(r);
													
						}
						else if(r["request"] == "main")
						{
							
							request_main_response(r);
													
						}
						else
						{
							
							message("error", r["error_message"]);
							
							log("    > ERROR [#apicom003] : Unknown response error.");
						
						}
						
						*/
												
					}					
					
					log("\r\n== END apicom response ==\r\n");
					
				}
						
				
			}// end yield
		
        } // end request
    			
	}
	
	private void login_response(SimpleJSON.JSONNode r){
		
		log("\r\n    == INIT login_response ==\r\n");
		
			token = r["token"];
			log("    > Token set to : "+token);
		
			id_user = r["id_user"];
			log("    > id_user set to : "+id_user);
			
			DBReader reader = db.Select("Select * from users WHERE id_user ='" + id_user + "' LIMIT 1");
			
			last_update = "0";
			
			if (reader != null && reader.Read())
			{

				last_update = reader.GetStringValue("last_update");

			}else{
				
				int result = db.Insert("INSERT INTO users VALUES ('"+db_lastid("users")+"','"+id_user+"','0') ");

				log("        > INSERT DB users: "+id_user+" - Result:  " + result );
					
				
			}
			
			log("    > last_update set to : "+last_update);
		
			message("loading", "Updating information ...");
		
			string json = "{\"request\": \"update\",\"token\": \""+token+"\",\"last_update\": \""+last_update+"\"} ";		
				
			StartCoroutine(apicom(json));
						
		log("\r\n    == END login_response ==\r\n");
				
	}
	
	private void update_response(SimpleJSON.JSONNode r)
	{
		
		log("\r\n    == INIT update_response ==\r\n");
		
			int result = 0;
			
			var i = 0;
			
			int flag_nation = 0;
			int flag_menu = 0;
							
			if(r["nation_list"] != null){
				
					
					log("        > NATION LIST COUNT: " + r["nation_list"].Count );
					
					
					for(i = 0; i < r["nation_list"].Count; i++){
						
						flag_nation = 1;
						
						log("        > NATION : " + r["nation_list"][i]["name"] );
					
						if(current_nation == r["nation_list"][i]["id_nation"]){
							
							flag_menu = 1;
							
						}
						
						DBReader reader = db.Select("Select * from users_nations WHERE id_user ='" + id_user + "' AND id_nation ='"+r["nation_list"][i]["id_nation"]+"' LIMIT 1");
						
						if (reader != null && reader.Read())
						{

							result = db.Update("UPDATE users_nations SET last_update='" + r["nation_list"][i]["last_update"] + "', name='" + r["nation_list"][i]["name"] + "'  WHERE id = '" + reader.GetStringValue("id") + "'");

							log("        > UPDATE DB: " + r["nation_list"][i]["name"] + " - Result: " + result );
					
						}else{
							
							result = db.Insert("INSERT INTO users_nations VALUES ('"+db_lastid("users_nations")+"','"+id_user+"','" + r["nation_list"][i]["id_nation"]  + "','" + r["nation_list"][i]["last_update"]  + "','" + r["nation_list"][i]["name"]  + "') ");

							log("        > INSERT DB: " + r["nation_list"][i]["name"] + " - Result:  " + result );
														
						}

						result = db.Update("UPDATE nations_structure SET status='2' WHERE id_user = '" + id_user + "' AND id_nation = '"+r["nation_list"][i]["id_nation"]+"' ");

						log("        > UPDATE DB: status = 2 on all structure from nation: "+r["nation_list"][i]["id_nation"]+" - Result: " + result );
				
					
					}					
					
			}else{
				
				log("        > NATION NULL ");
						
			}
			
			if(r["nation_structure"] != null){
				
				log("        > NATION STRUCTURE COUNT: " + r["nation_structure"].Count );
										
				
				for(i = 0; i < r["nation_structure"].Count; i++){
					
					
					
					log("        > NATION STRUCTURE: " + r["nation_structure"][i]["id_menu"] );
									
					result = db.Insert("INSERT INTO nations_structure VALUES ('"+db_lastid("nations_structure")+"','"+id_user+"','" + r["nation_structure"][i]["id_nation"]  + "','1','" + r["nation_structure"][i]["name"]  + "','" + r["nation_structure"][i]["type"]  + "','" + r["nation_structure"][i]["type_id"]  + "','" + r["nation_structure"][i]["icon"]  + "','" + r["nation_structure"][i]["order"]  + "') ");
					
					log("        > INSERT DB: " + r["nation_structure"][i]["id_menu"] + " - Result:  " + result );
									
				
				}				
				
			}else{
				
				log("        > NATION STRUCTURE NULL ");
						
			}
			
			result = db.Update("UPDATE users SET last_update = '"+ r["last_update"] +"'  WHERE id_user = '" + id_user + "'");

			log("        > UPDATE DB: User[" + id_user + "] - Last update: "+ r["last_update"] +" = " + result );
				
						
			//// Load the main screen
			
			if(GameObject.Find("Login") != null){
				
				message("hide", "");
		
				lobby("hide");
					
				GameObject.Find("Screens").transform.Find("Sidebar").gameObject.SetActive(true);
				log("        > Sidebar set to active");

				Sidebar("Tab1");

				Load_menu("");	
				
				Load_nations();
				
			}
			
			if(flag_menu == 1){
				
				Load_menu("");	
				
			}
			
			if(flag_nation == 1){
				
				Load_nations();
				
			}
			
			message("hide", "");
		
			
			// Load_page("");
				
				
			/*
				
			lobby("Hide");
		
			message("hide", "");
					
			GameObject.Find("Screens").transform.Find("Main").gameObject.SetActive(true);
			log("    > Main set to active");
			
			GameObject.Find("Screens/Main/Main_content_page").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), Screen.height);
			log("    > Main_content_page size changed to : "+GameObject.Find("Screens/Main/Main_content_page").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens/Main/Main_content_page/Title").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), 43);
			log("    > Main_content/Title size changed to : "+GameObject.Find("Screens/Main/Main_content_page/Title").GetComponent<RectTransform>().sizeDelta);
			
			GameObject.Find("Screens/Main/Main_content_page/Content").GetComponent<RectTransform>().sizeDelta = new Vector2((float)(Screen.width-308.5), (float)(Screen.height-43.0));
			log("    > Main_content/Content size changed to : "+GameObject.Find("Screens/Main/Main_content_page/Content").GetComponent<RectTransform>().sizeDelta);
				
			main_screen = "Home";
			
			tab("Tab1");
			
			loadpage("townhall");
			
									
			load_menu("");
			
			load_nations();		

				
				result = db.Update("UPDATE users SET last_update = '"+ r["last_update"] +"'  WHERE id = '" + id_user + "'");

				// result = db.Update("UPDATE users SET last_update = '1'  WHERE id_user = '2'");

				log("        > UPDATE DB: User[" + id_user + "] - Last update: "+ r["last_update"] +" = " + result );
							
			*/
		
		log("\r\n    == END update_response ==\r\n");
		
	}

	private void join_nation_response(SimpleJSON.JSONNode r)
	{
		
		log("\r\n    == INIT join_nation_response ==\r\n");
		
			DBReader reader = db.Select("Select * from users WHERE id_user ='" + id_user + "' LIMIT 1");
			
			last_update = "0";
			
			if (reader != null && reader.Read())
			{

				last_update = reader.GetStringValue("last_update");

			}else{
				
				int result = db.Insert("INSERT INTO users VALUES ('"+db_lastid("users")+"','"+id_user+"','0') ");

				log("        > INSERT DB users: "+id_user+" - Result:  " + result );
					
				
			}
			
			log("    > last_update set to : "+last_update);
		
			message("loading", "Retrieving nation information ...");
		
			string json = "{\"request\": \"update\",\"token\": \""+token+"\",\"last_update\": \""+last_update+"\"} ";		
				
			StartCoroutine(apicom(json));
						
		
			/*
		
			int result = 0;
			
			var i = 0;
			
			// string local_user = "0";
							
			if(r["nation_list"] != null){
				
					
					log("        > NATION LIST COUNT: " + r["nation_list"].Count );
					
					
					for(i = 0; i < r["nation_list"].Count; i++){
						
						log("        > NATION : " + r["nation_list"][i]["name"] );
					
						
						DBReader reader = db.Select("Select * from users_nations WHERE id_user ='" + id_user + "' AND id_nation ='"+r["nation_list"][i]["id_nation"]+"' LIMIT 1");
						
						
						if (reader != null && reader.Read())
						{

							result = db.Update("UPDATE users_nations SET last_update='" + r["nation_list"][i]["last_update"] + "', name='" + r["nation_list"][i]["name"] + "'  WHERE id = '" + reader.GetStringValue("id") + "'");

							log("        > UPDATE DB: " + r["nation_list"][i]["name"] + " - Result: " + result );
					
						}else{
							
							result = db.Insert("INSERT INTO users_nations VALUES ('"+db_lastid("users_nations")+"','"+id_user+"','" + r["nation_list"][i]["id_nation"]  + "','" + r["nation_list"][i]["last_update"]  + "','" + r["nation_list"][i]["name"]  + "') ");

							log("        > INSERT DB: " + r["nation_list"][i]["name"] + " - Result:  " + result );
														
						}					
					
					}					
					
			}else{
				
				log("        > NATION NULL ");
						
			}
			
						
			
			if(r["nation_structure"] != null){
				
				log("        > NATION STRUCTURE COUNT: " + r["nation_structure"].Count );
				
				// result = db.Update("UPDATE nations_structure SET status='2' WHERE id_user = '" + id_user + "'");

				// log("        > UPDATE DB: status = 2 on all structure - Result: " + result );
								
				
				for(i = 0; i < r["nation_structure"].Count; i++){
						
					log("        > NATION STRUCTURE: " + r["nation_structure"][i]["id_menu"] );
									
					result = db.Insert("INSERT INTO nations_structure VALUES ('"+db_lastid("nations_structure")+"','"+id_user+"','" + r["nation_structure"][i]["id_nation"]  + "','1','" + r["nation_structure"][i]["name"]  + "','" + r["nation_structure"][i]["type"]  + "','" + r["nation_structure"][i]["type_id"]  + "','" + r["nation_structure"][i]["icon"]  + "','" + r["nation_structure"][i]["order"]  + "') ");
					
					log("        > INSERT DB: " + r["nation_structure"][i]["id_menu"] + " - id_user : "+id_user+" - Result:  " + result );
									
				
				}				
				
			}else{
				
				log("        > NATION STRUCTURE NULL ");
						
			}
			
			Load_nations();
			
			message("hide", "");
			
			*/
			
			// result = db.Update("UPDATE users SET last_update = '"+ r["last_update"] +"'  WHERE id_user = '" + id_user + "'");

			// log("        > UPDATE DB: User[" + id_user + "] - Last update: "+ r["last_update"] +" = " + result );
				
						
			//// Load the main screen
			
			
			
		
			/*
			
			message("hide", "");
		
			lobby("hide");
				
			GameObject.Find("Screens").transform.Find("Sidebar").gameObject.SetActive(true);
			log("        > Sidebar set to active");

			Sidebar("Tab1");

			Load_menu("");	
			
			Load_nations();
			
			// Load_page("");
				
			*/	

		log("\r\n    == END join_nation_response ==\r\n");
		
	}
						
	// ***********************************
	// 		Support Functions
	// ***********************************
	
	// Helper function to set focus on input field
	private void focus(string target)
	{
		
		try
        {
			
			GameObject.Find(target).GetComponent<TMP_InputField>().Select();
			GameObject.Find(target).GetComponent<TMP_InputField>().ActivateInputField();

			
        }
        catch (Exception e)
        {
            
			error("Warning : Focus Object not found: "+target+"\r\nRaw Error: "+e+"");
			
        }
		
	}
		
	// Get the value of an input field
	private string get_value(string target)
	{
		
		try
        {
			
			return GameObject.Find(target).GetComponent<TMP_InputField>().text;
		
        }
        catch (Exception e)
        {
            
			error("Warning : Object not found to get value: "+target+"\r\nRaw Error: "+e+"");
			return null;
			
        }
		
	}
	
	// Set the value of an input field
	private void set_value(string target, string data)
	{
		
		try
        {
			
			GameObject.Find(target).GetComponent<TMP_InputField>().text = data;
			
        }
        catch (Exception e)
        {
            
			error("Warning : Object not found to set value: "+target+"\r\nRaw Error: "+e+"");
			
        }
		
	}
	
	// Get the value of an input field
	private void set_text(string target, string text)
	{
		
		try
        {
			
			GameObject.Find(target).GetComponent<TextMeshProUGUI>().text = text;
		
        }
        catch (Exception e)
        {
            
			error("Warning : Object not found to set text: "+target+"\r\nRaw Error: "+e+"");
			
        }
		
	}
	
	// Get the text from a text area
	private string get_text(string target)
	{
		
		try
        {
			
			return GameObject.Find(target).GetComponent<TextMeshProUGUI>().text;
			
        }
        catch (Exception e)
        {
            
			error("Warning : Object not found to get text: "+target+"\r\n"+e+"");
			return null;
			
        }		
		
	}
	
	// Saves a txt File
	private void savefile(string name, string text)
	{
		
		try
        {
			
			System.IO.File.WriteAllText( Application.persistentDataPath + "/"+name+".txt", text);
		}
        catch (Exception e)
        {
            
			error("Error : Impossible to create file: "+name+"\r\n"+e+"");
			
        }
		
	}
	
	// Load txt file
	private string loadfile(string name)
	{
		
		string response = "";
		
		try
        {
			
            StreamReader r = File.OpenText(  Application.persistentDataPath + "/"+name+".txt" ); 
			response = r.ReadToEnd(); 
			r.Close(); 
			
        }
        catch (Exception e)
        {
            
			error("Error : File not found: "+name+"\r\n"+e+"");
			
			response = null;
			
        }
		
		return response;
	   
	}
	
	// removes all child objects
	private void clear(string target)
	{
			
		if(GameObject.Find(target).gameObject.transform != null){
			
			Transform obj = GameObject.Find(target).gameObject.transform;

			foreach (Transform child in obj)
			{
				GameObject.Destroy(child.gameObject);
			}
			
		}
		else
		{
			
			error("Warning : Object not found to clear child: "+target+"\r\n");
						
		}		
		
	}
		
	// Return last id of a table	
	public int db_lastid(string name)
    {

        DBReader reader = db.Select("SELECT * FROM " + name + " ORDER BY id DESC LIMIT 1");
        
		if (reader != null && reader.Read())
        {
            
			return ( int.Parse( reader.GetStringValue("id") ) + 1 );
						
        }else{
			
			return 1;
			
		}

    }
	
	// Log Console Function
	private void log(string msg)
	{
		 
		Debug.Log(msg);

		if(GameObject.Find("Screens").transform.Find("Console/Text") != null)
		{
			
			GameObject.Find("Screens").transform.Find("Console/Text").GetComponent<TMP_InputField>().text += msg + "\r\n";	
					
		}
				
	}
	
	// Log Error on Console Function
	private void error(string msg)
	{
		 
		Debug.Log("********** "+msg+" **********");

		if(GameObject.Find("Screens").transform.Find("Console/Text") != null)
		{
			
			GameObject.Find("Screens").transform.Find("Console/Text").GetComponent<TMP_InputField>().text += "\r\n******************************\r\n"+msg+"\r\n******************************\r\n\r\n";	
					
		}
				
	}
	
	private void create_tables()
	{
		
		log("\r\n== INIT create_tables ==\r\n");
		
			// TABLE : Users

			DBSchema schema = new DBSchema("users");

			schema.AddField("id", SQLiteDB.DB_DataType.DB_INT, 0, false, true, true);
			schema.AddField("id_user", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("last_update", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
				   
			// create table
			bool result = db.CreateTable(schema); 

			log("    > Users table created? "+result);
			
			// TABLE : Users_nations

			schema = new DBSchema("users_nations");

			schema.AddField("id", SQLiteDB.DB_DataType.DB_INT, 0, false, true, true);
			schema.AddField("id_user", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("id_nation", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("last_update", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("name", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
				   
			// create table
			result = db.CreateTable(schema);

			log("    > Users_nations table created? "+result);
			
			// TABLE : Nations_structure

			schema = new DBSchema("nations_structure");

			schema.AddField("id", SQLiteDB.DB_DataType.DB_INT, 0, false, true, true);
			schema.AddField("id_user", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("id_nation", SQLiteDB.DB_DataType.DB_VARCHAR, 25, false, false, false);
			schema.AddField("status", SQLiteDB.DB_DataType.DB_VARCHAR, 1, false, false, false);
			schema.AddField("menu_name", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
			schema.AddField("menu_type", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
			schema.AddField("menu_type_id", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
			schema.AddField("menu_icon", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
			schema.AddField("menu_order", SQLiteDB.DB_DataType.DB_VARCHAR, 100, false, false, false);
				   
			// create table
			result = db.CreateTable(schema); 

			log("    > nations_structure table created? "+result);
			

		log("\r\n== END create_tables ==");
		
		
	}
	
}
