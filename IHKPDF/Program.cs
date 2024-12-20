using IHK;

var strCosmosIDarg = "";
var strREST_URLarg = "";
var strPathDataInputarg = "";

foreach (var strarg in args)
{
    if (strarg.ToLower().StartsWith("cosmosid=")) strCosmosIDarg = strarg.Substring(9);

    if (strarg.ToLower().StartsWith("server=")) strREST_URLarg = strarg.Substring(7);

    if (strarg.ToLower().StartsWith("verzeichnis=")) strPathDataInputarg = strarg.Substring(12);
}

var vv = new VorVerarbeitung(strCosmosIDarg, strREST_URLarg, strPathDataInputarg);