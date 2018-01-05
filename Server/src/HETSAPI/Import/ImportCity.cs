﻿using Hangfire.Console;
using Hangfire.Server;
using HETSAPI.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace HETSAPI.Import

{
    public class ImportCity
    {
        const string oldTable = "HETS_City";
        const string newTable = "HET_City";
        const string xmlFileName = "City.xml";
        const int sigId = 150000;

        static public void Import(PerformContext performContext, DbAppContext dbContext, string fileLocation, string systemId)
        {
        }

        /// <summary>
        /// Import existing Cities
        /// </summary>
        /// <param name="performContext"></param>
        /// <param name="dbContext"></param>
        /// <param name="fileLocation"></param>
        /// <param name="systemId"></param>
        static private void ImportCities(PerformContext performContext, DbAppContext dbContext, string fileLocation, string systemId)
        {
            string completed = DateTime.Now.ToString("d") + "-" + "Completed";
            ImportMap importMap = dbContext.ImportMaps.FirstOrDefault(x => x.OldTable == oldTable && x.OldKey == completed && x.NewKey == sigId);
            if (importMap != null)
            {
                performContext.WriteLine("*** Importing " + xmlFileName + " is complete from the former process ***");
                return;
            }
            try
            {
                string rootAttr = "ArrayOf" + oldTable;

                performContext.WriteLine("Processing " + oldTable);
                var progress = performContext.WriteProgressBar();
                progress.SetValue(0);

                // create serializer and serialize xml file
                XmlSerializer ser = new XmlSerializer(typeof(HETS_City[]), new XmlRootAttribute(rootAttr));
                MemoryStream memoryStream = ImportUtility.memoryStreamGenerator(xmlFileName, oldTable, fileLocation, rootAttr);
                HETS_City[] legacyItems = (HETS_City[])ser.Deserialize(memoryStream);
                foreach (var item in legacyItems.WithProgress(progress))
                {
                    // see if we have this one already.
                    importMap = dbContext.ImportMaps.FirstOrDefault(x => x.OldTable == oldTable && x.OldKey == item.City_Id.ToString());

                    if (importMap == null) // new entry
                    {
                        City city = null;
                        CopyToInstance(performContext, dbContext, item, ref city, systemId);
                        ImportUtility.AddImportMap(dbContext, oldTable, item.City_Id.ToString(), newTable, city.Id);
                    }
                    else // update
                    {
                        City city = dbContext.Cities.FirstOrDefault(x => x.Id == importMap.NewKey);
                        if (city == null) // record was deleted
                        {
                            CopyToInstance(performContext, dbContext, item, ref city, systemId);
                            // update the import map.
                            importMap.NewKey = city.Id;
                            dbContext.ImportMaps.Update(importMap);
                            dbContext.SaveChangesForImport();
                        }
                        else // ordinary update.
                        {
                            CopyToInstance(performContext, dbContext, item, ref city, systemId);
                            // touch the import map.
                            importMap.LastUpdateTimestamp = DateTime.UtcNow;
                            dbContext.ImportMaps.Update(importMap);
                            dbContext.SaveChangesForImport();
                        }
                    }
                }
                performContext.WriteLine("*** Done ***");
                ImportUtility.AddImportMap(dbContext, oldTable, completed, newTable, sigId);
            }

            catch (Exception e)
            {
                performContext.WriteLine("*** ERROR ***");
                performContext.WriteLine(e.ToString());
            }
        }


        /// <summary>
        /// Copy from legacy to new record For the table of HET_City
        /// </summary>
        /// <param name="performContext"></param>
        /// <param name="dbContext"></param>
        /// <param name="oldObject"></param>
        /// <param name="city"></param>
        /// <param name="systemId"></param>
        static private void CopyToInstance(PerformContext performContext, DbAppContext dbContext, HETS_City oldObject, ref Models.City city, string systemId)
        {
            bool isNew = false;
            if (city == null)
            {
                isNew = true;
                city = new City();
            }

            if (dbContext.Cities.Where(x => x.Name.ToUpper() == oldObject.Name.ToUpper()).Count() == 0)
            {
                isNew = true;
                city.Name = oldObject.Name.Trim();
                city.Id = dbContext.Cities.Max(x => x.Id) + 1;   //oldObject.Seq_Num;  
                city.CreateTimestamp = DateTime.UtcNow;
                city.CreateUserid = systemId;
            }

            if (isNew)
            {
                dbContext.Cities.Add(city);   //Adding the city to the database table of HET_CITY
            }

            try
            {
                dbContext.SaveChangesForImport();
            }
            catch (Exception e)
            {
                performContext.WriteLine("*** ERROR With add or update City ***");
                performContext.WriteLine(e.ToString());
            }
        }

    }
}