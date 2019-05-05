﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: May 5th 2019
 */
public class Inventory
{
    private static readonly string RESTORE_INVENTORY = "SELECT * FROM character_items WHERE owner=@owner";
    private static readonly string DELETE_INVENTORY = "DELETE FROM character_items WHERE owner=@owner";
    private static readonly string STORE_ITEM_START = "INSERT INTO character_items VALUES ";
    private readonly Dictionary<int, int> items = new Dictionary<int, int>();
    private readonly object itemLock = new object();

    public Inventory(string ownerName)
    {
        lock (itemLock)
        {
            // Restore information from database.
            try
            {
                MySqlConnection con = DatabaseManager.GetConnection();
                MySqlCommand cmd = new MySqlCommand(RESTORE_INVENTORY, con);
                cmd.Parameters.AddWithValue("owner", ownerName);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(reader.GetInt32("slot_id"), reader.GetInt32("item_id"));
                }
                con.Close();
            }
            catch (Exception e)
            {
                LogManager.Log(e.ToString());
            }
        }
    }

    // Only used when player exits the game.
    public void Store(string ownerName)
    {
        // Delete old records.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(DELETE_INVENTORY, con);
            cmd.Parameters.AddWithValue("owner", ownerName);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // No need to store if item list is empty.
        int itemCount = items.Count;
        if (itemCount == 0)
        {
            return;
        }

        // Prepare query.
        string query = STORE_ITEM_START;
        foreach (KeyValuePair<int, int> item in items)
        {
            query += "('" + ownerName + "'," + item.Key + "," + item.Value + ")" + (itemCount-- == 0 ? ";" : ",");
        }
        // Store new records.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Clear item list?
        // items.Clear();
    }

    public int GetSlot(int slotId)
    {
        return items[slotId];
    }

    public void SetSlot(int slotId, int itemId)
    {
        lock (itemLock)
        {
            items.Add(slotId, itemId);
        }
    }

    public void RemoveSlot(int slotId)
    {
        lock (itemLock)
        {
            items.Remove(slotId);
        }
    }

    public Dictionary<int, int> GetItems()
    {
        return items;
    }
}
