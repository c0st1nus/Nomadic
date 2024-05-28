using System;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class DataBaseHandler : MonoBehaviour
{
     string connStr = "server=20.52.101.204; uid=c0nstanta; database=Nomadic; pwd=03062008@rjitdjqK";
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
                     string username = reader.GetString("username");
                     string password = reader.GetString("password");
                     int balance = reader.GetInt32("balance");
                     int lives = reader.GetInt32("lives");
                     string UID = reader.GetString("UID");
                     string tgId = "";
                     try
                     {
                        tgId = reader.GetString("telegramID");
                     }
                     catch (Exception)
                     {
                         print("Telegram ID not found");
                     }
                     Data data = new Data(username, password, tgId, balance, lives, UID);
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
     
     public string UID { get; set; }
 
     public Data(string username, string password, string tgId, int balance, int lives, string UID)
     {
         this.Username = username;
         this.Password = password;
         this.TgId = tgId;
         this.Balance = balance;
         this.Lives = lives;
         this.UID = UID;
     }
 }
 