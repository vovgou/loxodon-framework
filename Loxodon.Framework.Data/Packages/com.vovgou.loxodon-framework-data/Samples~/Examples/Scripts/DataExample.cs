/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Loxodon.Framework.Examples.Repositories;
using System.IO;
using UnityEngine;
#if LITEDB
using LiteDB;
#endif

public class DataExample : MonoBehaviour
{
    void Start()
    {
#if NEWTONSOFT
        LoadFromJson();
#endif

#if LITEDB
        LoadFromLiteDB();
#endif
    }

#if NEWTONSOFT
    void LoadFromJson()
    {
        JsonEquipmentInfoRepository equipmentInfoRepository = new JsonEquipmentInfoRepository();

        var e1 = equipmentInfoRepository.GetById(1);
        Debug.LogFormat("LoadFromJson  id:{0} quality:{1} health:{2}", e1.Id, e1.Quality, e1.Health);
        var e2 = equipmentInfoRepository.GetBySign("equip001", 4);
        Debug.LogFormat("LoadFromJson  id:{0} quality:{1} health:{2}", e2.Id, e2.Quality, e2.Health);
    }
#endif

#if LITEDB
    private LiteDatabase database;

    private LiteDatabase LoadDatabase()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("LiteDB/data");
        return new LiteDatabase(new MemoryStream(textAsset.bytes));
    }

    private void OnDestroy()
    {
        if (database != null)
            database.Dispose();
    }

    void LoadFromLiteDB()
    {
        if (database == null)
            database = LoadDatabase();

        LiteDBEquipmentInfoRepository equipmentInfoRepository = new LiteDBEquipmentInfoRepository(database);

        var e1 = equipmentInfoRepository.GetById(1);
        Debug.LogFormat("LoadFromLiteDB id:{0} quality:{1} health:{2}", e1.Id, e1.Quality, e1.Health);
        var e2 = equipmentInfoRepository.GetBySign("equip001", 4);
        Debug.LogFormat("LoadFromLiteDB id:{0} quality:{1} health:{2}", e2.Id, e2.Quality, e2.Health);
    }
#endif
}
