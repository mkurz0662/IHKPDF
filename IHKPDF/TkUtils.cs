using System.Collections;
using net.compart.cplib.applog;
using net.compart.cplib.cpNetLib;
using net.compart.cplib.docponent;
using net.compart.cplib.docponenthelper;
using Utils;
using static System.Text.RegularExpressions.Regex;
using Index = net.compart.cplib.docponenthelper.Index;

namespace Toolkit;

public class TkUtils
{
    private const int bezugspunktAbs_x = 100;
    private const int bezugspunktAbs_y_2 = 300;
    private const int anschriftenzeilenAbstandY = 352;
    private const int senderlinesAbstand = 200;
    private const int dmcDiff = 2000;

    private readonly AddressParser adrParser;
    private readonly CpNetLib cpLib;

    private readonly Font fontAdress;
    private readonly Font fontSender;
    private readonly Frame frame;
    private readonly LogInstance logInstance;
    private readonly LogModule logModule;

    private readonly Length targetWindow_X_LEFT;
    private readonly Length targetWindow_X_RIGHT;
    private readonly Length targetWindow_Y_A;
    private readonly Length targetWindow_Y_B;
    private readonly Color textColor;

    public TkUtils(string profilespath, string logFile, string ftmgrfile) : this(profilespath, logFile, ftmgrfile,
        "Arial", "Arial", "10pt", "6pt")
    {
    }

    public TkUtils(string profilespath, string logFile, string ftmgrfile, string fontNameAdress,
        string fontNameSender) : this(
        profilespath, logFile, ftmgrfile, "Arial", "Arial", "10pt", "6pt")
    {
    }

    public TkUtils(string profilespath, string logFile, string ftmgrfile, string fontNameAdress, string fontNameSender,
        string adrSizeAdress, string adrSizeSender)
    {
        // Init Compart .NET Library
        cpLib = new CpNetLib();
        cpLib.SetTraceLevel(CpNetLib.TRACELEVEL.Warning);
        cpLib.SetTraceFileName(logFile + ".trc");
        logModule = new LogModule(cpLib);
        // init a first log instance from the log module. This may be done multiple times.
        logInstance = new LogInstance(logModule, "NET", logFile + ".log", LogInstance.INITFLAGS.Info);
        // init Framework for Docponents 

        frame = new Frame(logInstance, profilespath, "cpsdk.lic", ftmgrfile);
        //read password protected PDF 
        frame.UnregisterBreakMessage("PDI3003");
        frame.SetProcessingMode(Frame.ProcessingMode.RELAXED);


        // init Addressparser 
        adrParser = new AddressParser(frame, "DE");


        //init for addressShift
        var addXpos_B = new Length(frame, "9500tcm");
        targetWindow_X_LEFT = new Length(frame, "24mm");
        targetWindow_Y_B = new Length(frame, "50mm");
        targetWindow_Y_A = new Length(frame, "29mm");
        targetWindow_X_RIGHT = new Length(frame, "25mm");
        targetWindow_X_RIGHT.Add(addXpos_B);
        addXpos_B.Dispose();

        var senderSize = new Length(frame, adrSizeSender);
        var adrSize = new Length(frame, adrSizeAdress);
        fontSender = new Font(frame, fontNameSender, senderSize, Font.FontWeight.Medium, Font.FontStyle.Upright);
        fontAdress = new Font(frame, fontNameAdress, adrSize, Font.FontWeight.Medium, Font.FontStyle.Upright);


        senderSize.Dispose();
        adrSize.Dispose();

        textColor = new Color(frame);
        textColor.SetCMYK(0, 0, 0, 255);
    }

    public Frame GetFrame()
    {
        return frame;
    }

    public void closeTkUtils()
    {
        targetWindow_X_LEFT.Dispose();
        targetWindow_Y_B.Dispose();
        targetWindow_Y_A.Dispose();
        targetWindow_X_RIGHT.Dispose();
        fontAdress.Dispose();
        fontSender.Dispose();
        textColor.Dispose();
        adrParser.Dispose();
        frame.Dispose();
        logInstance.Dispose();
        logModule.Dispose();
        cpLib.Dispose();
    }

    public string CaptureText(Page page, string x, string y, string w, string h)
    {
        var xPos = new Length(frame, x);
        var yPos = new Length(frame, y);
        var width = new Length(frame, w);
        var height = new Length(frame, h);
        var capText = page.CaptureText(xPos, yPos, width, height, 50, Page.CaptureMode.AllIntersect |
                                                                      Page.CaptureMode.KeepOverprintedSpaces |
                                                                      Page.CaptureMode.PreserveLigature);
        var textArray = new ArrayList();
        for (var line = 0; line < capText.GetNumberOfLines(); line++) textArray.Add(capText.GetTextLine(line).Trim());
        try
        {
            xPos.Dispose();
            yPos.Dispose();
            width.Dispose();
            height.Dispose();
            capText.Dispose();
        }
        catch (CpLibException ex)
        {
            DPUtils.LogAdd(ex.ErrorMsg);
        }

        return string.Join(" ", textArray.ToArray());
    }

    //CaptureAdr
    public AdressData CaptureAdr(Page page, string x, string y, string w, string h, int senderLines, bool shiftAdr,
        bool RemoveEmptyLinesAlways, string foldposition)
    {
        var result = new AdressData();
        var xPos = new Length(frame, x);
        var yPos = new Length(frame, y);
        var width = new Length(frame, w);
        var height = new Length(frame, h);
        var capAdress = page.CaptureText(xPos, yPos, width, height, 50,
            Page.CaptureMode.AllIntersect | Page.CaptureMode.KeepOverprintedSpaces |
            Page.CaptureMode.PreserveLigature);

        if (senderLines > 0 || RemoveEmptyLinesAlways)
            RemoveEmptyLines(capAdress);

        if (shiftAdr)
        {
            page.EraseRect(xPos, yPos, width, height, Page.RemoveMode.AllIntersect);
            ShiftAdress(foldposition, "LEFT", page, capAdress, senderLines);
        }

        var tempAdr = new ArrayList();


        for (var line = senderLines; line < capAdress.GetNumberOfLines(); line++)
        {
            var crtLine = (senderLines == 0 ? line + 1 : line).ToString();
            tempAdr.Add(capAdress.GetTextLine(line));
        }


        //parse Address
        var adr = adrParser.Parse(capAdress, true);

        result.ZIP = adr.Get(Address.Component.Zip);
        result.CITY = adr.Get(Address.Component.City);
        result.STREET = adr.Get(Address.Component.Street);
        result.ISOCODE = adr.Get(Address.Component.CountryCode);
        result.COUNTRY = !string.IsNullOrWhiteSpace(adr.Get(Address.Component.CountryLine))
            ? adr.Get(Address.Component.CountryLine).ToUpper()
            : "DEUTSCHLAND";

        result.ADDRESSLINE1 = tempAdr != null && tempAdr.Count >= 1 ? tempAdr[0].ToString() : "";
        result.ADDRESSLINE2 = tempAdr != null && tempAdr.Count >= 2 ? tempAdr[1].ToString() : "";
        result.ADDRESSLINE3 = tempAdr != null && tempAdr.Count >= 3 ? tempAdr[2].ToString() : "";
        result.ADDRESSLINE4 = tempAdr != null && tempAdr.Count >= 4 ? tempAdr[3].ToString() : "";
        result.ADDRESSLINE5 = tempAdr != null && tempAdr.Count >= 5 ? tempAdr[4].ToString() : "";
        result.ADDRESSLINE6 = tempAdr != null && tempAdr.Count >= 6 ? tempAdr[5].ToString() : "";
        result.ADDRESSLINE7 = tempAdr != null && tempAdr.Count >= 7 ? tempAdr[6].ToString() : "";
        //result.Add("ISOCODE",
        //    !string.IsNullOrWhiteSpace(adr.Get(Address.Component.CountryCode))
        //        ? adr.Get(Address.Component.CountryCode)
        //        : "DE");

        try
        {
            adr.Dispose();
            capAdress.Dispose();
            xPos.Dispose();
            yPos.Dispose();
            width.Dispose();
            height.Dispose();
        }
        catch (CpLibException ex)
        {
            DPUtils.LogAdd(ex.ErrorMsg);
        }

        return result;
    }

    public void ShiftAdress(string foldPosition, string adrPos, Page page, ItemCollection capAdress, int senderLines)
    {
        var basePosY = foldPosition == "B" ? targetWindow_Y_B.GetTcm() : targetWindow_Y_A.GetTcm();

        var basePosX = adrPos == "LEFT" ? targetWindow_X_LEFT.GetTcm() : targetWindow_X_RIGHT.GetTcm();

        for (var x = 0; x < capAdress.GetNumberOfLines(); x++)
            if (senderLines > 0 && x == 0)
            {
                for (var y = x; y < senderLines; y++)
                {
                    var txt1 = new Text(frame, capAdress.GetTextLine(y), fontSender);
                    txt1.SetColor(textColor);
                    var newPosX = new Length(frame, basePosX + bezugspunktAbs_x, Length.Lengthunits.Tcm);
                    var newPosY = new Length(frame,
                        basePosY + bezugspunktAbs_y_2 + senderlinesAbstand * (y - senderLines), Length.Lengthunits.Tcm);
                    page.Put(txt1, newPosX, newPosY);
                    newPosX.Dispose();
                    newPosY.Dispose();
                    txt1.Dispose();
                }

                x = senderLines - 1;
            }
            else
            {
                var txt1 = new Text(frame, capAdress.GetTextLine(x), fontAdress);
                txt1.SetColor(textColor);
                var adrPosX = new Length(frame, basePosX + bezugspunktAbs_x, Length.Lengthunits.Tcm);
                var adrPosY = new Length(frame, basePosY + dmcDiff + anschriftenzeilenAbstandY * (x - senderLines),
                    Length.Lengthunits.Tcm);
                page.Put(txt1, adrPosX, adrPosY);
                adrPosX.Dispose();
                adrPosY.Dispose();
                txt1.Dispose();
            }
    }

    // RemoveEmptyLines
    public static void RemoveEmptyLines(ItemCollection addressLines)
    {
        var numberOfLines = addressLines.GetNumberOfLines();
        var index = 0;
        while (index < numberOfLines)
        {
            var line = addressLines.GetTextLine(index).Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                addressLines.RemoveItemByIndex(index);
                numberOfLines--;
            }
            else
            {
                index++;
            }
        }
    }

    public Hashtable readIndex(Page page, int iPage, bool readNops)
    {
        MetaData mdp;
        mdp = page.GetMetaData();
        var tleMap = new Hashtable();

        tleMap.Add("CP_PageNumber", iPage);
        tleMap.Add("CP_PapierInTray", Replace(page.GetPaperInTray(), "[^0-9]+", ""));
        tleMap.Add("CP_AfpCopyGroup", page.GetAfpCopyGroup());
        tleMap.Add("CP_SimplexDuplex", page.GetSimplexDuplex());
        tleMap.Add("CP_IsFrontPage",
            page.GetSimplexDuplex() != Page.SimplexDuplex.DuplexBack &&
            page.GetSimplexDuplex() != Page.SimplexDuplex.DuplexBackShortEdge);
        //tleMap.put("CP_IsEmptyPage", page.isEmpty());
        var iItem = -1;
        var crtNop = 0;
        while (mdp.HasItem(++iItem))
        {
            var item = mdp.GetItem(iItem);
            var classID = item.GetClassID();
            if (classID == Docponent.ClassID.IndexItem)
            {
                var idx = (IndexItem)item;
                tleMap.Add(idx.GetName(), idx.GetValue());
                idx.Dispose();
            }
            else if (readNops && classID == Docponent.ClassID.Comment)
            {
                var cmt = (Comment)item;

                tleMap.Add("NOP_" + ++crtNop, cmt.GetString());
                cmt.Dispose();
            }

            item.Dispose();
        }

        mdp.Dispose();
        return tleMap;
    }

    public Page MakeContainer(Page page, int pageNbr)
    {
//        var oc = page.HasTransparency()
//            ? new ObjectContainer(frame, ObjectContainer.ObjectType.PdfSinglePageWithTransparency)
//            : new ObjectContainer(frame, ObjectContainer.ObjectType.PdfSinglePageWithoutTransparency);
        var oc = new ObjectContainer(frame, ObjectContainer.ObjectType.PdfSinglePageWithTransparency);
        oc.SetName("O" + pageNbr.ToString().PadLeft(7, '0'));
        oc.SetTarget(ObjectContainer.Target.InPage);
        var ext = oc.Append(page);
        var pw = page.GetWidth();
        var ph = page.GetHeight();
        var objPage = new Page(frame, pw, ph);
        objPage.SetSimplexDuplex(page.GetSimplexDuplex());
        objPage.Put(ext);

        pw.Dispose();
        ph.Dispose();
        ext.Dispose();
        oc.Dispose();

        return objPage;
    }

    public void MovePage(Page page, string x, string y)
    {
        var XShift = new Length(frame, x);
        var YShift = new Length(frame, y);

        page.SetShift(XShift, YShift);

        XShift.Dispose();
        YShift.Dispose();
    }

    public bool IsEmptyArea(Page page, string x, string y, string w, string h)
    {
        var blnEmpty = false;
        var xPos = new Length(frame, x);
        var yPos = new Length(frame, y);
        var width = new Length(frame, w);
        var height = new Length(frame, h);
        blnEmpty = page.IsEmpty(xPos, yPos, width, height);
        xPos.Dispose();
        yPos.Dispose();
        width.Dispose();
        height.Dispose();

        return blnEmpty;
    }

    public void DeleteArea(Page page, string x, string y, string w, string h)
    {
        var xPos = new Length(frame, x);
        var yPos = new Length(frame, y);
        var width = new Length(frame, w);
        var height = new Length(frame, h);
        page.EraseRect(xPos, yPos, width, height, Page.RemoveMode.AllIntersect);
        xPos.Dispose();
        yPos.Dispose();
        width.Dispose();
        height.Dispose();
    }

    public void PutAdress(string foldPosition, string adrPos, Page page, List<string> strAdresslines,
        string strSenderline)
    {
        var basePosY = foldPosition == "B" ? targetWindow_Y_B.GetTcm() : targetWindow_Y_A.GetTcm();

        var basePosX = adrPos == "LEFT" ? targetWindow_X_LEFT.GetTcm() : targetWindow_X_RIGHT.GetTcm();

        var senderLines = 0;

        if (strSenderline != string.Empty)
        {
            var txts = new Text(frame, strSenderline, fontSender);
            txts.SetColor(textColor);
            var newPosX = new Length(frame, basePosX + bezugspunktAbs_x, Length.Lengthunits.Tcm);
            var newPosY = new Length(frame, basePosY + bezugspunktAbs_y_2, Length.Lengthunits.Tcm);
            page.Put(txts, newPosX, newPosY);
            newPosX.Dispose();
            newPosY.Dispose();
            txts.Dispose();
        }

        for (var x = 0; x < strAdresslines.Count; x++)
        {
            var txt1 = new Text(frame, strAdresslines[x], fontAdress);
            txt1.SetColor(textColor);
            var adrPosX = new Length(frame, basePosX + bezugspunktAbs_x, Length.Lengthunits.Tcm);
            var adrPosY = new Length(frame, basePosY + dmcDiff + anschriftenzeilenAbstandY * (x - senderLines),
                Length.Lengthunits.Tcm);
            page.Put(txt1, adrPosX, adrPosY);
            adrPosX.Dispose();
            adrPosY.Dispose();
            txt1.Dispose();
        }
    }

    public void PutAbsender(Page page, string strSenderline, double dblX, double dblY)
    {
        var txts = new Text(frame, strSenderline, fontSender);
        txts.SetColor(textColor);
        var newPosX = new Length(frame, dblX, Length.Lengthunits.Tcm);
        var newPosY = new Length(frame, dblY, Length.Lengthunits.Tcm);
        page.Put(txts, newPosX, newPosY);
        newPosX.Dispose();
        newPosY.Dispose();
        txts.Dispose();
    }

    public bool CheckFontsEmbedded(Page page, string strFilename, int intPageNumber)
    {
        var blnEmbedded = true;

        try
        {
            var fontnames = new List<string>();
            var fontnamesnotembedded = new List<string>();
            var fontlistembedding = new List<string>();
            var IC = page.CaptureFonts();
            var numFonts = IC.GetNumberOfFonts();
            for (var i = 0; i < numFonts; i++)
            {
                var pFont = IC.GetFont(i);
                var strFontname = pFont.GetName();
                var blnIsSubsetted = pFont.IsSubset();
                var blnIsEmbedded = pFont.IsEmbedded();
                pFont.Dispose();
                if (fontnames.Contains(strFontname) == false) fontnames.Add(strFontname);
                if (fontnamesnotembedded.Contains(strFontname) == false) fontnamesnotembedded.Add(strFontname);
                if (blnIsEmbedded) fontlistembedding.Add(strFontname + "_" + "EMBEDDED");
                else fontlistembedding.Add(strFontname + "_" + "NOEMBEDDED");
            }

            foreach (var fontname in fontnames)
                if (fontlistembedding.Contains(fontname + "_" + "EMBEDDED"))
                    fontnamesnotembedded.Remove(fontname);
            if (fontnamesnotembedded.Count > 0) blnEmbedded = false;
        }
        catch (Exception ex)
        {
            blnEmbedded = false;
        }

        return blnEmbedded;
    }

    public void SetPageSizeFormatA4P(Page page)
    {
        page.SetPageSizeFormat(Page.Paperformat.A4, Page.Orientation.Portrait);
    }

    public void Rotate2Portrait(Page page)
    {
        page.SetRotationMode(Page.RotationMode.Portrait);
    }

    public void TLEAdd(Index idx, string strIndex, string strValue)
    {
        var idxitem = new IndexItem(frame, strIndex, strValue);
        //idxitem.SetPositionType(pos);
        idx.AddIndexItem(idxitem);
        idxitem.Dispose();
    }


    public void SeiteVerschieben(Page page, string x, string y)
    {
        var XShift = new Length(frame, x);
        var YShift = new Length(frame, y);

        page.SetShift(XShift, YShift);

        XShift.Dispose();
        YShift.Dispose();
    }

    public ExternalItem SetOverlay(string fileIn, string ovlname, int pageIdx, int rotation, string scaleW,
        string scaleH, bool transparency)
    {
        var fileInputDocument = new FileInputDocument(frame, fileIn);
        var externalItemOverlay = fileInputDocument.ReadPage(pageIdx);

        if (!string.IsNullOrEmpty(scaleW) || !string.IsNullOrEmpty(scaleW))
        {
            var w = new Length(frame, scaleW);
            var h = new Length(frame, scaleH);
            externalItemOverlay.ScaleToImage(w, h, 300, 300);
            w.Dispose();
            h.Dispose();
        }

        if (rotation != 0) externalItemOverlay.SetRotation(rotation);

        if (transparency) externalItemOverlay.SetTransparency(transparency, Page.ItemType.All);

        var ext = new ExternalItem(frame, ovlname, ExternalItem.Type.Overlay);
        ext.AttachOverlay(externalItemOverlay);

        externalItemOverlay.Dispose();
        fileInputDocument.Dispose();
        return ext;
    }

    public void ConvertFile(string inFilename, string outFilename, string inType, string outType, bool convertGray,
        string greyIcc, bool isDuplex, bool textToImage = false, bool sepOverlays = false, bool makeObj = false, bool mergeFonts = false, bool mergeText = false, bool toImage = false)
    {
        var inAttr = new InputDocumentAttributes(frame);
        inAttr.SetType(inType);
        var outAttr = new OutputDocumentAttributes(frame);
        outAttr.SetSelfContained(true);
        outAttr.SetSeparateOverlays(sepOverlays);
       
        outAttr.SetType(outType);
        if (mergeFonts) outAttr.SetMergeFonts(mergeFonts, mergeFonts);

        var inFile = new FileInputDocument(frame, inFilename, inAttr);
        var outFile = new FileOutputDocument(frame, outFilename, outAttr);

        var pageCount = 0;

        while (inFile.HasPage(++pageCount))
        {
            var page = inFile.ReadPage(pageCount);
            if (isDuplex)
            {
                if (pageCount % 2 == 0)  page.SetSimplexDuplex(Page.SimplexDuplex.DuplexBack);
                else page.SetSimplexDuplex(Page.SimplexDuplex.DuplexFront);
            }
            else page.SetSimplexDuplex(Page.SimplexDuplex.Simplex);
            if (pageCount % 1000 == 0)
                DPUtils.LogAdd($"Convert File Process Pages {pageCount}");

            if (mergeText)
                page.SetMergeText(Page.MergeText.PreserveItemOrder | Page.MergeText.PreserveCharacterSpacing |
                                  Page.MergeText.WordSeparator);

            if (page.IsLandscape()) page.SetRotationMode(Page.RotationMode.Portrait);
            if (convertGray ) page.ApplyIccProfile(greyIcc);
            if (textToImage )  page.ConvertItemsToImage(Page.ItemType.Text, 300, 300, true);
            if (toImage )  page.ConvertToImage(true,300,300);
            if (makeObj && !textToImage)
            {
                Page objPage = MakeContainer(page, pageCount);
                outFile.Write(objPage);
                objPage.Dispose();
            }
            else outFile.Write(page);

            page.Dispose();
        }

        inAttr.Dispose();
        outAttr.Dispose();
        inFile.Close();
        outFile.Close();
        inFile.Dispose();
        outFile.Dispose();
    }
}
public class AdressData
{
    public string ZIP { get; set; } = string.Empty; //City
    public string COUNTRY { get; set; } = string.Empty; //COUNTRY
    public string ISOCODE { get; set; } = string.Empty; //ISOCODE
    public string CITY { get; set; } = string.Empty; // CITY
    public string STREET { get; set; } = string.Empty; // Street
    public string? ADDRESSLINE1 { get; set; } = string.Empty; // ADDRESSLINE1
    public string? ADDRESSLINE2 { get; set; } = string.Empty; // ADDRESSLINE2
    public string ADDRESSLINE3 { get; set; } = string.Empty; // ADDRESSLINE3
    public string ADDRESSLINE4 { get; set; } = string.Empty; // ADDRESSLINE4
    public string ADDRESSLINE5 { get; set; } = string.Empty; // ADDRESSLINE5
    public string ADDRESSLINE6 { get; set; } = string.Empty; // ADDRESSLINE6
    public string ADDRESSLINE7 { get; set; } = string.Empty; // ADDRESSLINE7
}