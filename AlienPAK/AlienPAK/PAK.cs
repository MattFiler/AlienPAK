﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienPAK
{
    /*
     *
     * Our PAK handler.
     * Created by Matt Filer: http://www.mattfiler.co.uk
     * 
     * Intended to support various PAK formats for Alien: Isolation (CATHODE).
     * Currently a WORK IN PROGRESS.
     * 
    */
    class PAK
    {
        ToolOptionsHandler ToolSettings = new ToolOptionsHandler();
        AnyPAK PAKHandler;
        public PAKType Format = PAKType.UNRECOGNISED;

        /* Open a PAK archive */
        public PAKReturnType Open(string FilePath)
        {
            //Set PAKHandler to appropriate class depending on input filename
            switch (Path.GetFileName(FilePath))
            {
                case "GLOBAL_TEXTURES.ALL.PAK":
                case "LEVEL_TEXTURES.ALL.PAK":
                    PAKHandler = new TexturePAK(FilePath);
                    Format = PAKType.PAK_TEXTURES;
                    break;
                case "GLOBAL_MODELS.PAK":
                case "LEVEL_MODELS.PAK":
                    PAKHandler = new ModelPAK(FilePath);
                    Format = PAKType.PAK_MODELS;
                    break;
                case "MATERIAL_MAPPINGS.PAK":
                    PAKHandler = new MaterialMapPAK(FilePath);
                    Format = PAKType.PAK_MATERIALMAPS;
                    break;
                case "COMMANDS.PAK":
                    PAKHandler = new CommandPAK(FilePath);
                    Format = PAKType.PAK_SCRIPTS;
                    break;
                default:
                    PAKHandler = new PAK2(FilePath);
                    Format = PAKType.PAK2;
                    break;
            }

            //Attempt to load, and if it fails, set format to UNRECOGNISED
            PAKReturnType LoadReturnInfo = PAKHandler.Load();
            switch (LoadReturnInfo)
            {
                case PAKReturnType.SUCCESS_WITH_WARNINGS:
                case PAKReturnType.SUCCESS:
                    return LoadReturnInfo;
                default:
                    Format = PAKType.UNRECOGNISED;
                    return LoadReturnInfo;
            }
        }

        /* Parse a PAK archive */
        public List<string> Parse()
        {
            return PAKHandler.GetFileNames();
        }

        /* Get the size of a file within the PAK archive */
        public int GetFileSize(string FileName)
        {
            return PAKHandler.GetFilesize(FileName);
        }

        /* Export from a PAK archive */
        public PAKReturnType ExportFile(string FileName, string ExportPath)
        {
            return PAKHandler.ExportFile(ExportPath, FileName);
        }

        /* Import to a PAK archive */
        public PAKReturnType ImportFile(string FileName, string ImportPath)
        {
            //PAK2 is the only type to currently support full archive handling with the Save() method
            if (Format == PAKType.PAK2)
            {
                PAKReturnType ReplaceFilePAK2 = PAKHandler.ReplaceFile(ImportPath, FileName);
                if (ReplaceFilePAK2 == PAKReturnType.SUCCESS)
                {
                    return PAKHandler.Save();
                }
                return ReplaceFilePAK2;
            }

            return PAKHandler.ReplaceFile(ImportPath, FileName);
        }

        /* Remove from a PAK archive */
        public PAKReturnType RemoveFile(string FileName)
        {
            if (Format != PAKType.PAK2) { return PAKReturnType.FAIL_FEATURE_IS_COMING_SOON; } //Currently only supported in PAK2
            PAKReturnType DeleteFilePAK2 = PAKHandler.DeleteFile(FileName);
            if (DeleteFilePAK2 == PAKReturnType.SUCCESS)
            {
                return PAKHandler.Save();
            }
            return DeleteFilePAK2;
        }

        /* Add to a PAK archive */
        public PAKReturnType AddNewFile(string NewFile)
        {
            if (Format != PAKType.PAK2) { return PAKReturnType.FAIL_FEATURE_IS_COMING_SOON; } //Currently only supported in PAK2
            PAKReturnType AddFilePAK2 = PAKHandler.AddFile(NewFile);
            if (AddFilePAK2 == PAKReturnType.SUCCESS)
            {
                return PAKHandler.Save();
            }
            return AddFilePAK2;
        }
    }
}
