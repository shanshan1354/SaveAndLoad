using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.IO;
using LitJson;
using System.Xml;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    public bool isPause = true;
    public GameObject menuGO;
    public GameObject[] targetArray;

    private string path;

    private void Awake()
    {
        _instance = this;
        Cursor.visible = true;
        Time.timeScale = 0;
        isPause = true;
        path = Application.dataPath + "/StreamingAssets";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    //将需要保存的数据存储到save中
    private Save CreateSaveGO()
    {
        Save save = new Save();
        foreach (GameObject target in targetArray)
        {
            TargetManager targetManager = target.GetComponent<TargetManager>();
            if (targetManager.activeMonster != null)
            {
                save.livingTargetPositions.Add(targetManager.targetPosition);
                int type = targetManager.activeMonster.GetComponent<MonsterManager>().monsterType;
                save.livingMonsterType.Add(type);
            }
        }
        save.score = UIManager.Instance.scoreNum;
        save.shootNum = UIManager.Instance.shootNum;
        return save;
    }
    //将保存的数据设置到游戏中
    private void SetGame(Save save)
    {
        foreach (GameObject target in targetArray)
        {
            target.GetComponent<TargetManager>().UpdateMonsters();
        }
        for (int i = 0; i < save.livingTargetPositions.Count; i++)
        {
            int targetPosition = save.livingTargetPositions[i];
            int monsterType = save.livingMonsterType[i];
            targetArray[targetPosition].GetComponent<TargetManager>().ActivateMonsterByType(monsterType);
        }
        UIManager.Instance.scoreNum = save.score;
        UIManager.Instance.shootNum = save.shootNum;
        UIManager.Instance.LoadUI();
    }

    //二进制方法保存加载游戏数据
    private void SaveByBin()
    {
        //创建一个二进制格式器
        BinaryFormatter bf = new BinaryFormatter();
        //创建一个可对文件进行操作的文件流
        FileStream fileStream = File.Create(path + "/ByBin.txt");
        //将save数据保存到文件中
        Save save = CreateSaveGO();
        bf.Serialize(fileStream, save);
        //关闭文件流
        fileStream.Close();
    }
    
    private void LoadByBin()
    {
        if (File.Exists(path + "/ByBin.txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(path + "/ByBin.txt", FileMode.Open);
            Save save = (Save)bf.Deserialize(fileStream);
            fileStream.Close();
            SetGame(save);
            ContinueGame();
            UIManager.Instance.CloseMessage();
        }
        else
        {
            UIManager.Instance.ShowMessage("没有存档可加载");
        }
    }

    //xml方法保存加载游戏数据
    private void SaveByXml()
    {
        Save save = CreateSaveGO();
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement root = xmlDocument.CreateElement("save");
        root.SetAttribute("name","file1");
        for (int i = 0; i < save.livingTargetPositions.Count; i++)
        {
            XmlElement targetGO = xmlDocument.CreateElement("target");
            
            XmlElement targetPos = xmlDocument.CreateElement("targetPosition");
            targetPos.InnerText = save.livingTargetPositions[i].ToString();
            targetGO.AppendChild(targetPos);
            XmlElement monsterPos = xmlDocument.CreateElement("monsterPosition");
            monsterPos.InnerText = save.livingMonsterType[i].ToString();
            targetGO.AppendChild(monsterPos);
            root.AppendChild(targetGO);
        }
        
        XmlElement shootNum = xmlDocument.CreateElement("shootNum");
        shootNum.InnerText = save.shootNum.ToString();
        root.AppendChild(shootNum);
        XmlElement score = xmlDocument.CreateElement("score");
        score.InnerText = save.score.ToString();
        root.AppendChild(score);

        xmlDocument.AppendChild(root);
        xmlDocument.Save(path+"/ByXml.txt");
    }

    private void LoadByXml()
    {
        if (File.Exists(path+"/ByXml.txt"))
        {
            Save save = new Save();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path + "/ByXml.txt");
            XmlNodeList targetList= xmlDocument.GetElementsByTagName("target");
            XmlNodeList targetPosList = xmlDocument.GetElementsByTagName("targetPosition");
            XmlNodeList monsterPosList= xmlDocument.GetElementsByTagName("monsterPosition");
            for (int i = 0; i < targetList.Count; i++)
            {
                save.livingTargetPositions.Add(int.Parse(targetPosList[i].InnerText));
                save.livingMonsterType.Add(int.Parse(monsterPosList[i].InnerText));
            }
            XmlNodeList shootNumList = xmlDocument.GetElementsByTagName("shootNum");
            save.shootNum = int.Parse(shootNumList[0].InnerText);
            XmlNodeList scoreList = xmlDocument.GetElementsByTagName("score");
            save.score = int.Parse(scoreList[0].InnerText);
            SetGame(save);

            ContinueGame();
            UIManager.Instance.CloseMessage();
        }
        else
        {
            UIManager.Instance.ShowMessage("没有存档可加载");
        }
    }

    //json方法保存加载游戏数据
    private void SaveByJson()
    {
        Save save = CreateSaveGO();
        string str = JsonMapper.ToJson(save);
        StreamWriter streamWriter = new StreamWriter(path + "/ByJson.json");
        streamWriter.Write(str);
        streamWriter.Close();
    }

    private void LoadByJson()
    {
        if (File.Exists(path+"/ByJson.json"))
        {
            StreamReader streamReader = new StreamReader(path + "/ByJson.json");
            string str = streamReader.ReadToEnd();
            Save save = JsonMapper.ToObject<Save>(str);
            streamReader.Close();
            SetGame(save);
            ContinueGame();
            UIManager.Instance.CloseMessage();
        }
        else
        {
            UIManager.Instance.ShowMessage("没有存档可加载");
        }
    }

    //暂停游戏
    public void Pause()
    {
        isPause = true;
        Cursor.visible = true;
        Time.timeScale = 0;
        menuGO.SetActive(true);
    }

    //开始新游戏
    public void NewGame()
    {
        ContinueGame();
        UIManager.Instance.UpdateUI();
        foreach (GameObject target in targetArray)
        {
            target.GetComponent<TargetManager>().UpdateMonsters();
        }
        UIManager.Instance.CloseMessage();
    }

    //继续游戏
    public void ContinueGame()
    {
        isPause = false;
        Cursor.visible = false;
        Time.timeScale = 1;
        menuGO.SetActive(false);
        UIManager.Instance.CloseMessage();
    }
    //保存游戏
    public void SaveGame()
    {
        //SaveByBin();
        //SaveByJson();
        SaveByXml();
        UIManager.Instance.ShowMessage("保存成功");
    }

    //加载游戏
    public void LoadGame()
    {
        //LoadByBin();
        //LoadByJson();
        LoadByXml();
    }

    //退出游戏
    public void ExitGame()
    {
        Application.Quit();
    }
}
