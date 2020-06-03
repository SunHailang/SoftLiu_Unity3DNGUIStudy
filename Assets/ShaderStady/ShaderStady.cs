using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ShaderStady : MonoBehaviour {

    public static void MinifyFile (string path) {
        Debug.Log ("Minifying JSON file " + path);

        string contents = File.ReadAllText (path);
        contents = Regex.Replace (contents, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

        File.WriteAllText (path, contents);
    }

    // Start is called before the first frame update
    void Start () {

        string fileName = "orderFile.json";
        string path = System.IO.Path.Combine (Application.persistentDataPath, fileName);
        if (File.Exists (path)) {

            MinifyFile (path);

            string info = null;
            using (StreamReader sr = new StreamReader (path)) {
                info = sr.ReadToEnd ();
            }

            Debug.Log ("read: " + info);
            if (!string.IsNullOrEmpty (info)) {
                Dictionary<string, object> data1 = MiniJSON.Deserialize (info) as Dictionary<string, object>;
                if (data1 != null && data1.ContainsKey ("result")) {
                    List<object> data2 = data1["result"] as List<object>;
                    if (data2 != null) {
                        Debug.Log ("data2: " + data2.Count);
                        foreach (var item in data2) {
                            if (item == null) continue;
                            Dictionary<string, object> dicItem = item as Dictionary<string, object>;
                            string createtime = dicItem["createtime"] as string;
                            string timeStamp = dicItem["timeStamp"] as string;
                            string actualcost = dicItem["actualcost"] as string;
                            string propid = dicItem["propid"] as string;
                            string orderid = dicItem["orderid"] as string;
                        }
                    }
                }
            }
        }

        Dictionary<string, object> dic = new Dictionary<string, object> ();
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>> ();
        dic.Add ("result", list);
        Dictionary<string, object> dicInfo = new Dictionary<string, object> ();
        dicInfo.Add ("createtime", DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"));
        dicInfo.Add ("timeStamp", "");
        dicInfo.Add ("actualcost", "");
        dicInfo.Add ("propid", "");
        dicInfo.Add ("orderid", "");
        list.Add (dicInfo);
        Dictionary<string, object> dicInfo1 = new Dictionary<string, object> ();
        dicInfo1.Add ("createtime", DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss"));
        dicInfo1.Add ("timeStamp", "");
        dicInfo1.Add ("actualcost", "");
        dicInfo1.Add ("propid", "");
        dicInfo1.Add ("orderid", "");
        list.Add (dicInfo1);

        string json = MiniJSON.Serialize (dic);
        Debug.Log (json);

    }

    // Update is called once per frame
    void Update () {

    }
}