using Newtonsoft.Json.Linq;

namespace MTools
{
    class GetMcSounds
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入 Minecraft 路径（路径最后不用带 \\）：");
            string minecraftPath = Console.ReadLine();
            Console.WriteLine("请输入json版本(如1.8.json)");
            string jsonFilePath = Path.Combine(minecraftPath, "assets", "indexes", Console.ReadLine());
            Console.WriteLine("正在解析 JSON 文件...");
            var (hashes, names) = AnalyseJson(jsonFilePath);

            Console.WriteLine("解析成功！");
            Console.WriteLine("请输入导出路径：");
            string exportPath = Console.ReadLine()+Path.DirectorySeparatorChar;

            for (int i = 0; i < hashes.Length; i++)
            {
                string objectsPath = Path.Combine(minecraftPath, "assets", "objects");
                string sourceFilePath = Path.Combine(objectsPath, TakeString(hashes[i]), hashes[i]);
                Path.Combine(); 

                string targetFilePath = Path.Combine(exportPath, names[i]);
                string FolderPath = "";
                string[] folderList = names[i].Split("/");
                for (int j = 0; j <= folderList.Length-2; j++)
                    FolderPath += folderList[j] + Path.DirectorySeparatorChar;
                string targetFolderPath = exportPath + FolderPath;
                Console.WriteLine(targetFolderPath);
                CreateFolder(exportPath, targetFolderPath);
                CopyFile(sourceFilePath, exportPath+names[i]);

                int progress = (i + 1) * 100 / hashes.Length;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"Progress: [{new string('#', progress / 2)}{new string(' ', 50 - progress / 2)}] {progress}%");
            }
            Console.WriteLine("\n操作完成！");
        }

        static (string[] hash, string[] name) AnalyseJson(string jsonPath)
        {
            string jsonContent = File.ReadAllText(jsonPath);
            JObject data = JObject.Parse(jsonContent);
            JObject objects = (JObject)data["objects"];

            List<string> names = new List<string>();
            List<string> hashes = new List<string>();

            foreach (var item in objects)
            {
                string key = item.Key;
                JObject value = (JObject)item.Value;
                names.Add(key);
                hashes.Add((string)value["hash"]);
            }

            return (hashes.ToArray(), names.ToArray());
        }

        static void CopyFile(string sourcePath, string targetPath)
        {
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath);
            }
        }

        static string TakeString(string str)
        {
            return str.Length < 2 ? str : str.Substring(0, 2);
        }

        static void CreateFolder(string path, string str)
        { 
            path = Path.Combine(path,str);
            Directory.CreateDirectory(path);
        }
    }
}
