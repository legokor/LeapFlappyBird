using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
public struct TopListItem
{
    public int score;
    public string name;
}
public static class TopList
{
    public static HashSet<TopListItem> topList = new();

    public static void Load()
    {
        topList = new HashSet<TopListItem>();
        if (File.Exists(Application.persistentDataPath + "/toplistnamed.txt"))
        {
            string[] lines = File.ReadAllLines(Application.persistentDataPath + "/toplistnamed.txt");
            foreach (var line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length > 1)
                {
                    string name = parts[0];
                    int score = int.Parse(parts[1]);
                    topList.Add(new TopListItem() { name = name, score = score });
                }
            }
        }
    }

    public static void Save()
    {
        string[] lines = new string[topList.Count];
        int i = 0;
        foreach (var item in topList)
        {
            lines[i] = item.name + ";" + item.score;
            i++;
        }
        File.WriteAllLines(Application.persistentDataPath + "/toplistnamed.txt", lines);
    }
    public static bool GetsOnList(int score)
    {
        return topList.Count < 10 || score > topList.Min(x => x.score);
    }

    public static void Add(string name, int score)
    {
        topList.Add(new TopListItem() { name = name, score = score });
        if (topList.Count > 10)
        {
            topList.Remove(topList.Min());
        }
    }

    public static string GetList(){
        string list = "";
        List<TopListItem> tlist = topList.ToList<TopListItem>();
        int i = 1;
        foreach (var item in tlist.OrderByDescending(x => x.score))
        {
            list += i + ". " + item.name + " - " + item.score + "\n";
            i++;
        }
        return list;
    }

}
