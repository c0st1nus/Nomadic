using System;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class DataBaseHandler : MonoBehaviour
{
     private readonly string connStr = "server=145.249.249.29; uid=remoteuser; database=mydb; pwd=userpassword";
     private Sprite ConvertLongBlobToSprite(byte[] imageBytes)
     {
         if (imageBytes == null) return null;
         Texture2D texture = new Texture2D(2, 2);
         texture.LoadImage(imageBytes);
         return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
     }
     public Data[] Select(string query = "SELECT * FROM users")
     {
         List<Data> dataList = new List<Data>();
         try
         {
             using (MySqlConnection connection = new MySqlConnection(connStr))
             {
                 connection.Open();
 
                 MySqlCommand command = new MySqlCommand(query, connection);
                 MySqlDataReader reader = command.ExecuteReader();
 
                 while (reader.Read())
                 {
                     string UID = reader.GetString("UID");
                     string username = reader.GetString("name");
                     string password = reader.GetString("pass");
                     string tgId = reader.GetString("t_id");
                     int balance = reader.GetInt32("bal");
                     int lives = reader.GetInt32("liv");
                     byte[] avatarBytes;
                     avatarBytes = (byte[])reader["av"];
                     Sprite avatar = ConvertLongBlobToSprite(avatarBytes);
                     Data data = new Data(username, password, tgId, balance, lives, UID, avatar);
                     dataList.Add(data);
                 }
             
                 connection.Close();
             }
 
             return dataList.ToArray();
         }
         catch (MySqlException e)
         {
             Debug.LogError(e);
             return null;
         }
     }
 }
 
 public class Data
 {
     public int Id { get; set; }
     public string Username { get; set; }
     public string Password { get; set; }
     public string TgId { get; set; }
     public int Balance { get; set; }
     public int Lives { get; set; }
     public Sprite Avatar { get; set; }
     public string UID { get; set; }
 
     public Data(string username, string password, string tgId, int balance, int lives, string UID, Sprite avatar = null)
     {
         this.Username = username;
         this.Password = password;
         this.TgId = tgId;
         this.Balance = balance;
         this.Lives = lives;
         this.Avatar = avatar;
         this.UID = UID;
     }
 }
 