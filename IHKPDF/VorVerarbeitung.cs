using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using net.compart.cplib.docponent;
using Sdi;
using Toolkit;
using Utils;
using Path = System.IO.Path;

namespace IHK;

public class VorVerarbeitung
{
    private const bool convertGray = false;
    private const bool convertToImage = false;
    private const bool makeContainer = false;
    private const string afpExtension = ".afp";
    private const string pdfExtension = ".pdf";
    private const string jsonExtension = ".json";

    private readonly Frame frame = null!;
    private SpooldataInterface spoolData = new SpooldataInterface()
    
    {
        filetype = "PDF",
        spooltype = "ESTAMPING",
        doctype = "A4",
        docclass = "Briefe",
        prodtype = "TEST",
        mandant = "IHK",
        provider = "DPAG",
        iscutsheet = false,
        usecostunit = true
    };
 
    //Compart Toolkit + Whitepooljason
    private readonly TkUtils tkutils;
    private readonly string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
    private readonly string strLogfile = Environment.CurrentDirectory + @"\" + "toolkit_";
    private readonly string processLogfile = Environment.CurrentDirectory + @"\" + "preprocessing_";
    private readonly string strICCGenericGray = Environment.CurrentDirectory + @"\" + "icc" +@"\" + "GenericGray.icc";
    private readonly string strICCRgb = Environment.CurrentDirectory + @"\" + "icc" +@"\" + "GenericRGB.icc";


    //Pfade
    
    private readonly string strToolkitProfilePath = Environment.CurrentDirectory + @"\profile\";

    private readonly string strTyp = "BRIEFE";

    public VorVerarbeitung(string strCosmosIDarg, string strREST_URLarg, string strPathDataInputarg)
    {
        try
        {
            DPUtils.strLogPath = processLogfile + timestamp + ".log";
            if (strCosmosIDarg == "")
            {
                REST_POST(RESTPostStateTypes.Error, "Fehler CosmosID wurde nicht übergeben", "");
                DPUtils.LogAdd("CosmosID nicht per Kommand-Parameter übergeben");
                Environment.Exit(1);
            }

            if (strREST_URLarg == "")
            {
                REST_POST(RESTPostStateTypes.Error, "Fehler Server wurde nicht übergeben", "");
                DPUtils.LogAdd("Server nicht per Kommand-Parameter übergeben");
                Environment.Exit(1);
            }

            if (strPathDataInputarg == "")
            {
                REST_POST(RESTPostStateTypes.Error, "Fehler Verzeichnis wurde nicht übergeben", "");
                DPUtils.LogAdd("Verzeichnis nicht per Kommand-Parameter übergeben");
                Environment.Exit(1);
            }

            var ftMgrFile = Environment.CurrentDirectory + @"\ftmgr" + timestamp + ".xml";
            tkutils = new TkUtils(strToolkitProfilePath, strLogfile + timestamp, ftMgrFile);
            frame = tkutils.GetFrame();


            if (strPathDataInputarg.EndsWith(@"\"))
                strPathDataInputarg = strPathDataInputarg.Substring(0, strPathDataInputarg.Length - 1);

            COSMOSID = strCosmosIDarg;
            SERVER_RESTURL = strREST_URLarg;
            INPUTPATH = strPathDataInputarg;
            Init();
            RunToolkitIHK();
            File.Delete(ftMgrFile);
        }
        catch (Exception ex)
        {
            DPUtils.LogAdd("Fehler:" + ex.Message);
        }
    }

    private string INPUTPATH { get; } = string.Empty;
    private string OUTPUTPATH { get; set; } = string.Empty;
    private string BASEOUTPUT { get; set; } = string.Empty;
    private string INPUTPATHTEMP { get; set; } = string.Empty;
    private string COSMOSID { get; } = string.Empty;
    private string SERVER_RESTURL { get; } = string.Empty;
    private string SYSTEMTYPE { get; set; } = string.Empty;


    private void Init()
    {
        REST_POST(RESTPostStateTypes.Update, "Preprocessing Init " + COSMOSID, "");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var dirInfo = Directory.GetParent(Directory.GetParent(INPUTPATH)!.FullName);
        SYSTEMTYPE = dirInfo!.Name;
        BASEOUTPUT = Path.Combine(dirInfo.FullName, "Output", COSMOSID);
        OUTPUTPATH = Path.Combine(BASEOUTPUT, "temp");
        INPUTPATHTEMP = Path.Combine(INPUTPATH, "temp");
        if (Directory.Exists(BASEOUTPUT)) new DirectoryInfo(BASEOUTPUT).Delete(true);
        if (!Directory.Exists(OUTPUTPATH)) Directory.CreateDirectory(OUTPUTPATH);
    }

    private void RunToolkitIHK()
    {
        try
        {
            REST_POST(RESTPostStateTypes.Update, "Preprozessing GetShipments " + COSMOSID, "");
            var listAllInputFiles = GetAllInputFiles(INPUTPATH);

            var strMandant = spoolData.mandant;

            REST_POST(RESTPostStateTypes.Update, "Preprozessing ProcessData " + COSMOSID, "");

            ProcessData(listAllInputFiles, strMandant);
            tkutils.closeTkUtils();

            frame.Dispose();
            REST_POST(RESTPostStateTypes.Update, "Preprozessing ZIPData " + COSMOSID, "");

            DataZIPAndCleanup(strMandant);

            REST_POST(RESTPostStateTypes.ProcessEnd, "Preprocessing Fertig", BASEOUTPUT);

            DPUtils.LogAdd("Ende Preprocessing");
        }
        catch (Exception ex)
        {
            REST_POST(RESTPostStateTypes.Error, "Fehler beim Preprozessing " + ex.Message, "");
            DPUtils.LogAdd("Fehler beim Preprozessing " + ex.Message);
        }
    }
  
    private void ProcessData(List<string> listFiles, string strMandant)
    {
        var inAttr = new InputDocumentAttributes(frame);
        inAttr.SetType("PDF");
        
        var outAttr = new OutputDocumentAttributes(frame);
        outAttr.SetSelfContained(true);
        outAttr.SetType("PDF");
        outAttr.SetMergeFonts(true,true);

        var bundling = new Dictionary<string, string>();
        foreach (var file in listFiles)
        {
            //spoolData.PRODTYPE = SYSTEMTYPE is "TEST" or "DEV" ? "TEST" : "PROD";
            spoolData.prodtype = "PROD";
            spoolData.docclass = strTyp;
            spoolData.mailpieces = new List<SpooldataMailpiece>();
            var strOutputFilename =
                OUTPUTPATH + "\\" + strMandant + "_" + Guid.NewGuid() + pdfExtension;

            var outFile = new FileOutputDocument(frame, strOutputFilename, outAttr);
            var arlPagesOut = new ArrayList();

            arlPagesOut.Clear();

            var fi = new FileInputDocument(frame, file, inAttr);

            var intGlobalPageCounterOutput = 0;
            var pageCounterInStart = 1;
            var pageCountDocument = 0;
 
            var pageCount = 0;

            SpooldataMailpiece jsonMailpiece = null!;
            SpooldataDocument jsonDocument = null!;

            IHKDDL ddlData = null!;
            var outPages = 0;
            while (fi.HasPage(++pageCount))
            {
                intGlobalPageCounterOutput++;
                if (intGlobalPageCounterOutput % 1500 == 0)
                    REST_POST(RESTPostStateTypes.Update, "Process Pages " + intGlobalPageCounterOutput,
                        "");

                var page = fi.ReadPage(pageCount);

                AdressData jsonAdress = null!;
                
                var pageDDL = tkutils.CaptureText(page, "0mm", "0mm", "8mm", "290mm");

                var docFirstPage = pageDDL != null && !string.IsNullOrEmpty(pageDDL) &&
                                   pageDDL.Contains("$DDLSTART$", StringComparison.Ordinal);

                tkutils.MovePage(page, "0mm", "0.7mm");

                if (docFirstPage)
                {
                    if (outPages > 0)
                    {
                        jsonMailpiece.custdocid = ddlData.DOKID;
                        if (!bundling.ContainsKey(jsonMailpiece.custdocid))
                        {
                            var bundleid = Guid.NewGuid().ToString();
                            bundling.Add(jsonMailpiece.custdocid, bundleid);
                            jsonMailpiece.bundleid = bundling[jsonMailpiece.custdocid];
                        }
                        else
                        {
                            jsonMailpiece.bundleid = bundling[jsonMailpiece.custdocid];
                        }

                        //jsonMailpiece.BUNDLE = ddlData.DOKID;
                        jsonDocument.pages = pageCountDocument;
                        jsonDocument.sheets = (int)(ddlData.PRINTMODE == "SIMPLEX"
                            ? pageCountDocument
                            : Math.Ceiling((double)pageCountDocument / 2));
                        jsonDocument.pageto = intGlobalPageCounterOutput - 1;
                        jsonMailpiece.documents.Add(jsonDocument);
                        spoolData.mailpieces.Add(jsonMailpiece);

                        OuputPages(arlPagesOut, outFile,ddlData.PRINTMODE == "SIMPLEX");
                        arlPagesOut.Clear();
                    }

                    ddlData = IHKDDL.ProcessDDL(pageDDL!);
                    if (pageCount == 1)
                    {
                        spoolData.docclass = ddlData.PACKAGEID;
                        spoolData.pdfprintmode = ddlData.PRINTMODE;
                    }


                    outPages = 1; 
                    pageCountDocument = 1;

                    jsonMailpiece = new SpooldataMailpiece()
                    {
                        digitalcopy = false,
                        custmandant = ddlData.KOSTENSTELLE,
                        zip = ddlData.ZIP,
                        isocode = ddlData.ISOCODE
                    };
                    jsonDocument = new SpooldataDocument()
                    {
                        color = ddlData.COLOR,
                        plexmode = ddlData.PRINTMODE,
                        copies = 0,
                        isDuplex = ddlData.PRINTMODE == "DUPLEX"
                    };

                    //ProcessAddressData(jsonAdress, jsonMailpiece);

                    var CUSTDOCID_UUID = Guid.NewGuid();
                    jsonMailpiece.custdocid = ddlData.DOKID;
                    jsonMailpiece.poolfilename = Path.GetFileName(strOutputFilename);
                    jsonDocument.dokumentId = Guid.NewGuid().ToString();
                    jsonMailpiece.billref = ddlData.KOSTENSTELLE;

                    jsonMailpiece.priority = 1;
                    jsonDocument.orderinshipment = 1;
                    jsonDocument.pagefrom = intGlobalPageCounterOutput;
                    jsonDocument.infilename = Path.GetFileName(file);
                }
                else
                {
                    outPages++;
                    pageCountDocument++;
                }

                arlPagesOut.Add(page.Clone());
                page.Dispose();
               
            }

            fi.Close();
            
            fi.Dispose();

            if (!bundling.ContainsKey(jsonMailpiece.custdocid))
            {
                var bundleid = Guid.NewGuid().ToString();
                bundling.Add(jsonMailpiece.custdocid, bundleid);
                jsonMailpiece.bundleid = bundling[jsonMailpiece.custdocid];
            }
            else
            {
                jsonMailpiece.bundleid = bundling[jsonMailpiece.custdocid];
            }

            jsonDocument.doctype = spoolData.doctype;
            jsonDocument.pages = pageCountDocument;
            jsonDocument.sheets = ddlData.PRINTMODE == "SIMPLEX" ? pageCountDocument : pageCountDocument / 2;
            jsonDocument.pageto = intGlobalPageCounterOutput;
            jsonMailpiece.documents.Add(jsonDocument);
            spoolData.mailpieces.Add(jsonMailpiece);
            OuputPages(arlPagesOut, outFile,ddlData.PRINTMODE == "SIMPLEX" );
            arlPagesOut.Clear();
            outFile.Close();
            outFile.Dispose();
            var options = new JsonSerializerOptions { WriteIndented = true };
            var strJSONInfo = JsonSerializer.Serialize(spoolData, options);
            if (spoolData.mailpieces.Count > 0)
                File.WriteAllText(strOutputFilename.Replace(pdfExtension, jsonExtension), strJSONInfo);
        }
        
        outAttr.Dispose();
        inAttr.Dispose();
    }

    /**
     * DDL für Rechnung usw
     */
    public void ProcessAddressData(AdressData jsonAdress, SpooldataMailpiece jsonMailpiece)
    {
        jsonMailpiece.adrLine1 = jsonAdress.ADDRESSLINE1.Trim();
        jsonMailpiece.adrLine2 = jsonAdress.ADDRESSLINE2.Trim();
        jsonMailpiece.adrLine3 = jsonAdress.ADDRESSLINE3.Trim();
        jsonMailpiece.adrLine4 = jsonAdress.ADDRESSLINE4.Trim();
        jsonMailpiece.adrLine5 = jsonAdress.ADDRESSLINE5.Trim();
        jsonMailpiece.adrLine6 = jsonAdress.ADDRESSLINE6.Trim();
        //jsonMailpiece.ZIP = jsonAdress.ZIP.Trim();
        jsonMailpiece.country = jsonAdress.COUNTRY.Trim();
        jsonMailpiece.city = jsonAdress.CITY.Trim();
        jsonMailpiece.street = jsonAdress.STREET.Trim();
        //jsonMailpiece.ISOCODE = string.Empty;
    }

    /**
     * DDL für Rechnung usw
     */
    public void OuputPages(ArrayList arlPagesOut, FileOutputDocument outFile, bool isSimplex)
    {
        var pageNbr = 0;
        foreach (Page outPage in arlPagesOut)
        {
            ++pageNbr;
            if (convertToImage)  outPage.ConvertItemsToImage(Page.ItemType.Text, 300, 300, false);
            outFile.Write(outPage);
            outPage.Dispose();
        }
    }


    public List<string> GetAllInputFiles(string strInputPath)
    {
        var fileArrayInput = Directory.GetFiles(strInputPath);
        var listFiles = new List<string>();
        foreach (var file in fileArrayInput) listFiles.Add(file);

        return listFiles;
    }

    public void DataZIPAndCleanup(string strMandant)
    {
        var strTimestampGenerate = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var strZIPOutfilename = OUTPUTPATH + @"\" + strMandant + "_" + strTimestampGenerate + ".7z";

        DPUtils.LogAdd("7ZIP Start");
        DPUtils.Compress_7ZIP(strZIPOutfilename, OUTPUTPATH); //Alternative zu 7 ZIP

        DPUtils.LogAdd("move 7ZIP");
        File.Move(strZIPOutfilename,
            BASEOUTPUT + @"\" + Path.GetFileName(strZIPOutfilename));

        DPUtils.LogAdd("Delete Temp Files in " + OUTPUTPATH);
        //Temp Dateien löschen
        foreach (var strFilename in Directory.GetFiles(OUTPUTPATH)) File.Delete(strFilename);

        DPUtils.LogAdd("Delete Outputpath " + OUTPUTPATH);
        //Temp-Ordner löschen
        Directory.Delete(OUTPUTPATH);

        DPUtils.LogAdd("Delete Inputpath " + INPUTPATH);
        //Datain-Ordner löschen rekursiv da es noch Unterordner geben kann
        Directory.Delete(INPUTPATH, true);
    }

    private void REST_POST(string strState, string strMessage, string strVerzeichnis)
    {
        var tsk = REST_POSTTask(strState, strMessage, strVerzeichnis);
        tsk.Wait();
    }

    private async Task REST_POSTTask(string strState, string strMessage, string strVerzeichnis)
    {
        try
        {
            var jsonPost = new JsonRESTpost();
            jsonPost.COSMOSID = COSMOSID;
            jsonPost.SERVER = Environment.MachineName;
            jsonPost.STATE = strState;
            jsonPost.MESSAGE = strMessage;
            jsonPost.VERZEICHNIS = strVerzeichnis;

            var client = new HttpClient();
            client.BaseAddress = new Uri(SERVER_RESTURL);

            var byteArray = Encoding.ASCII.GetBytes("DaPaREST:admin1234");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Add("AuthProvider", "cosmos-webapi");

            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonData = JsonSerializer.Serialize(jsonPost, options);
            var RestContent =
                new StringContent(jsonData, Encoding.UTF8, "application/json");
            var RestResponse = await client.PostAsync(SERVER_RESTURL, RestContent);

            var strResult = RestResponse.StatusCode.ToString();
            DPUtils.LogAdd("REST Response POST " + strState + "   " + strMessage + "   " + strResult + "   " +
                           strVerzeichnis + "   " + SERVER_RESTURL);

            client.Dispose();
        }

        catch (Exception ex)
        {
            DPUtils.LogAdd("REST_POST Fehler " + ex.Message);
        }
    }
}

public class IHKDDL
{
    public string DDL { get; set; } = string.Empty;
    public string ISOCODE { get; set; } = string.Empty;
    public string ZIP { get; set; } = "0";
    public string EKP { get; set; } = string.Empty;
    public string VERFAHREN { get; set; } = string.Empty;
    public string KOSTENSTELLE { get; set; } = string.Empty;
    public string DOKID { get; set; } = string.Empty;
    public string EMPFID_PREM { get; set; } = string.Empty;
    public string EKP_PREM { get; set; } = string.Empty;
    public string ADRID_PREM { get; set; } = string.Empty;
    public string PAGES { get; set; } = string.Empty;
    public string PACKAGEID { get; set; } = string.Empty;
    public string DDLVERSION { get; set; } = string.Empty;
    public string PREMIUMPRODUKT { get; set; } = string.Empty;
    public string BEILAGE { get; set; } = string.Empty;
    public string BEILAGE_MANDATORY { get; set; } = string.Empty;
    public string BEILAGE_PDF { get; set; } = string.Empty;
    public string ANREDE { get; set; } = string.Empty;
    public string VORNAME { get; set; } = string.Empty;
    public string NACHNAME { get; set; } = string.Empty;
    public string STRASSE { get; set; } = string.Empty;
    public string HAUSNUMMER { get; set; } = string.Empty;
    public string ORT { get; set; } = string.Empty;
    public string FIRMENNAME { get; set; } = string.Empty;
    public string BUNDLEID { get; set; } = string.Empty;
    public string PAKETBUNDLEID { get; set; } = string.Empty;
    public string ADRBUNDLEID { get; set; } = string.Empty;
    public string CLOSEENVELOPE { get; set; } = string.Empty;
    public string F27 { get; set; } = string.Empty;
    public string F28 { get; set; } = string.Empty;
    public string PRINTMODE { get; set; } = string.Empty;
    public string GLOBALINSERT { get; set; } = string.Empty;
    public bool HASCOLOR { get; set; }
    public string COLOR { get; set; } = string.Empty;

    public static IHKDDL ProcessDDL(string strDDL)
    {
        var result = new IHKDDL();
        var startString = "$DDLSTART$";
        var endString = "$DDLEND$";
        // $$START$$;RE01348148288;;;;;45134;;DEUTSCHLAND;;;;;7;;;;;;;;;;;;;;$$END$$
        if (!string.IsNullOrEmpty(strDDL) && strDDL.Contains(startString))
        {
            var inString = strDDL.Replace("\"", string.Empty);

            var startPos = inString.IndexOf(startString) + startString.Length;
            var endPos = inString.IndexOf(endString);
            var processString = string.Empty;
            processString = startPos != -1 ? inString.Substring(startPos, endPos - startPos) : inString;
            var splittedDDL = processString.Split(";");
            result.ZIP = "0";
            result.DDL = processString.Trim();
            result.ISOCODE = splittedDDL[0].Trim();
            result.ZIP = splittedDDL[1].Trim();
            result.EKP = splittedDDL[2].Trim();
            result.VERFAHREN = splittedDDL[3].Trim();
            result.KOSTENSTELLE = splittedDDL[4].Trim();
            result.DOKID = splittedDDL[5].Trim();
            result.EMPFID_PREM = splittedDDL[6].Trim();
            result.EKP_PREM = splittedDDL[7].Trim();
            result.ADRID_PREM = splittedDDL[8].Trim();
            result.PAGES = splittedDDL[9].Trim();
            result.PACKAGEID = splittedDDL[10].Trim().ToUpper();
            result.DDLVERSION = splittedDDL[11].Trim();
            result.PREMIUMPRODUKT = splittedDDL[12].Trim();
            result.BEILAGE = splittedDDL[13].Trim();
            result.BEILAGE_MANDATORY = splittedDDL[14].Trim();
            result.BEILAGE_PDF = splittedDDL[15].Trim();
            result.ANREDE = splittedDDL[16].Trim();
            result.VORNAME = splittedDDL[17].Trim();
            result.NACHNAME = splittedDDL[18].Trim();
            result.STRASSE = splittedDDL[19].Trim();
            result.HAUSNUMMER = splittedDDL[20].Trim();
            result.ORT = splittedDDL[21].Trim();
            result.FIRMENNAME = splittedDDL[22].Trim();
            result.BUNDLEID = splittedDDL[23].Trim();
            result.PAKETBUNDLEID = splittedDDL[24].Trim().ToUpper();
            result.ADRBUNDLEID = splittedDDL[25].Trim();
            result.CLOSEENVELOPE = splittedDDL[26].Trim();
            result.F27 = splittedDDL[27].Trim();
            result.F28 = splittedDDL[28].Trim();
            result.PRINTMODE = result.PACKAGEID.Contains("STD_02") || result.PACKAGEID.Contains("STD_04")
                ? "DUPLEX"
                : "SIMPLEX";

            if (result.PACKAGEID.Contains("STD_01") || result.PACKAGEID.Contains("STD_02")) result.HASCOLOR = false;
            else result.HASCOLOR = true;
            if (result.PACKAGEID.Contains("STD_01")) result.COLOR = "1/0";
            else if (result.PACKAGEID.Contains("STD_02")) result.COLOR = "1/1";
            else if (result.PACKAGEID.Contains("STD_03")) result.COLOR = "4/0";
            else result.COLOR = "4/4";

            if (result.BEILAGE == "J")
                result.GLOBALINSERT = result.BEILAGE.Substring(0, result.BEILAGE.IndexOf(".")) +
                                      (result.BEILAGE_MANDATORY == "J" ? "MANDATORY" : "");
        }

        return result;
    }
}