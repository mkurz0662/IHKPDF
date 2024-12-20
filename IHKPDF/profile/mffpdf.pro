<?xml version="1.0" encoding="UTF-8"?>

<mffpdf xmlns="http://www.compart.com/ns/mff/pdf" version="202409">
   <!--
      **************************************************************************

      Copyright (C) Compart AG 2011-2022, Compart GmbH 2022

      Release: 202409

      **************************************************************************
-->
   <!--A replacement character that is to be used if the requested character is
                not included in the font. It must be contained in the WinAnsi character set.-->

   <defaultchar char="003F"/>

   <!--Font list. Its attributes apply to fonts which are not
                explicitly included in the profile or which do not specify a value for the
                respective attribute. Otherwise, specific font attribute values take precedence over
                font list attribute values. 
                "subsetting" - (only in output) - specifies, whether subsets of the embedded fonts 
                should be specified in the output files. Font subsets can be specified for Type 1 
                and TrueType fonts. -->


   <fontlist subsetting="true">
      
      <font family="Arial" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="MEDIUM" width="NORMAL" fontfile="_a______" fontfiletype="Type1" />
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="_ab_____" fontfiletype="Type1" />
         <face style="ITALIC" weight="MEDIUM" width="NORMAL" fontfile="_ai_____" fontfiletype="Type1" />
         <face style="ITALIC" weight="BOLD" width="NORMAL" fontfile="_abi____" fontfiletype="Type1" />
      </font>
      <font family="ArialMT">
         <face weight="Medium" style="Upright" fontfile="_a______" fontfiletype="Type1" />
      </font>
      <font family="Arial-BoldMT">
         <face weight="Bold" style="Upright" fontfile="_ab_____" fontfiletype="Type1" />
      </font>
      <font family="Arial,Bold">
         <face weight="Bold" style="Upright" fontfile="_ab_____" fontfiletype="Type1" />
      </font>
      <font family="ArialMT,Bold">
         <face weight="Bold" style="Upright" fontfile="_ab_____" fontfiletype="Type1" />
      </font>
      <font family="Arial-ItalicMT" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="ITALIC" weight="MEDIUM" width="NORMAL" fontfile="_ai_____" fontfiletype="Type1" />
      </font>
      <font family="Arial,Italic" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="ITALIC" weight="MEDIUM" width="NORMAL" fontfile="_ai_____" fontfiletype="Type1" />
      </font>
      <font family="Arial-BoldItalicMT" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="ITALIC" weight="BOLD" width="NORMAL" fontfile="_abi____" fontfiletype="Type1" />
      </font>

      <font family="Helvetica" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="MEDIUM" width="NORMAL" fontfile="_a______" fontfiletype="Type1" />
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="_ab_____" fontfiletype="Type1" />
      </font>
      <font family="Helvetica-Bold" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="_ab_____" fontfiletype="Type1" />
      </font>
      <font family="Helvetica-BoldOblique" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="ITALIC" weight="BOLD" width="NORMAL" fontfile="_abi____" fontfiletype="Type1" />
      </font>

      <font family="Helvetica-Oblique" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="ITALIC" weight="MEDIUM" width="NORMAL" fontfile="_ai_____" fontfiletype="Type1" />
      </font>
      <font family="Times" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="MEDIUM" width="NORMAL" fontfile="_er______" fontfiletype="Type1" />
      </font>
      <font family="Times-Roman" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="MEDIUM" width="NORMAL" fontfile="_er______" fontfiletype="Type1" />
      </font>
      <font family="Times-Bold" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="_eb_____" fontfiletype="Type1" />
      </font>
      <font family="Courier" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="com_____" fontfiletype="Type1" />
      </font>
      <font family="Courier-Bold" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="cob_____" fontfiletype="Type1" />
      </font>
      <font family="OpenSans" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="OpenSans" fontfiletype="Type1" />
      </font>
      <font family="OpenSans-Bold" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="OpenSans-Bold" fontfiletype="Type1" />
      </font>
      <font family="Calibri" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="calibri" fontfiletype="TrueType" />
      </font>
      <font family="Calibri-Bold" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="calibrib" fontfiletype="TrueType" />
      </font>
      <font family="Calibri-Italic" serifstyle="SANSSERIF" spacing="PROPORTIONAL">
         <face style="UPRIGHT" weight="BOLD" width="NORMAL" fontfile="calibrii" fontfiletype="TrueType" />
      </font>
  

   </fontlist>

   <!--The colorprofilelist element contains one colorprofile element
                per color space. Profiles in the colorprofile elements will be used as the input ICC for 
                a colorspace where an input item is specified as device colour. For PDF input files all 
                colorspaces that are expected in the input should have an ICC profile defined in the 
                colorprofilelist. For PDF output an output intent should also be defined, otherwise 
                unexpected results may occur, see outputintent element in this profile. 
                An ICC profile is a standardized record that defines the color space of a device. The ICC 
                profiles can either have the file extension icc (Windows) or icm (Mac). In addition to 
                specifying the file name of the color profile, you can use the resourcelist element 
                to specify which directories are to be searched for the specified color profiles -->
   <!--<colorprofilelist>        
      <colorprofile name="GenericRGB"/>        
      <colorprofile name="GenericCMYK"/>        
      <colorprofile name="GenericGray"/>
   </colorprofilelist>-->
   <!--Settings for input.-->

   <input>
      <!--If set to true, the occurrences of the PDF XForms which are
                not scaled, rotated or skewed (i.e. XForms having translation-only CTMs( will be
                read as overlays-->

      <overlayasreference value="false"/>

      <!--Global annotations treatment. Attributes: "read" - specifies
                whether annotations are read (TRUE) or not (FALSE). The default value is TRUE;
                "generate" - specifies whether annotations are replaced by the objects they
                represent (FALSE) or whether annotation-items are generated (TRUE). The default
                value is FALSE to ensure backward compatibility, since this setting suits most of
                the customers (as of august 2005). "printall": An easier way to set "forceprinting" 
                to TRUE for all annotation types (overrides the PDF noprinting flag if this is set 
                to TRUE).-->

      <annotations read="true" generate="false" generatefields="false" printall="false" generatearchivefields="true"/>
   </input>

   <!--Settings for output.-->

   <output>
      <!--Specifies the PDF output format. 
                Attributes: 
                "value": Specifies the PDF version that is to be created as output. The versions 1.3, 1.4,
                1.5, 1.6, 1.7, PDF/A-1, PDF/A-2, PDF/A-3, PDF/A-4, PDF/UA or combinations of them are
                supported. 
                "level": With PDF/A-1 as output format, you can specify here whether the
                created PDF files conform to PDF/A-1a (a) or PDF/A-1b (b). This setting is ignored
                if the version is any other than PDF/A, PDF/A-1, PDF/A-2 or PDF/A-3. For PDF/A-2 and PDF/A-3,
                in addition to 'a' and 'b', level 'u' may be specified to generate PDF/A-2u and PDF/A-3u.
                "strict": With PDF/A-1, PDF/A-2, PDF/A-3, PDF/A-4 or PDF/UA as output format, you can specify here whether
                the created files strictly conform to the standard. By setting strict="false" the
                process is not cancelled even if the output file does not comply to all PDF/A or
                PDF/UA requirements. By setting strict="true" however, the process is cancelled if
                no proper PDF/A or PDF/UA output file can be created. The setting is ignored if the
                version is any other than PDF/A and PDF/UA.                
                -->

      <version value="PDF/A-1" level="b" strict="false"/>

      <!--Specifies the compression level for the ZIP compression.
                Attributes: "level": Specifies the level of compression. Valid values range from 0
                (fastest, no compression) to 9 (highest compression); "purpose": Specifies the
                object to be ZIP-compressed. Currently this setting is ingored. The default value is
                IMAGE.-->

      <zipcompression level="default" purpose="Image"/>

      <!--OutputIntent. Attributes: "name": The value of "name" must be
                exactly one of the following: For RGB: "Adobe RGB (1998)" "Apple RGB" "ColorMatch
                RGB" "sRGB IEC61966-2.1" For CMYK: "Europe ISO Coated FOGRA27" "Euroscale Uncoated
                v2" "Japan Color 2001 Coated" "Japan Color 2001 Uncoated" "Japan Color 2002
                Newspaper" "Japan Web Coated (Ad)" "U.S. Sheetfed Coated v2" "U.S. Sheetfed Uncoated
                v2" "U.S. Web Coated (SWOP) v2" "U.S. Web Uncoated v2" If more than one
                outputintents are given only the first one is used while the rest are ignored.
                "filename": If present, it will override the value specified in name. It should be
                the file name of a valid, PDF/A conform ICC profile.-->

      <outputintent name="Adobe RGB (1998)" filename=""/>

      <!--Viewer. Attributes: "pagemode": Specifies whether the document
                is to be initially opened offering attachments display ("ATTACHMENTS"), thumbnail navigation ("THUMBS"), bookmark
                navigation ("OUTLINES") or in full screen mode ("FULLSCREEN"). The default value is
                "NONE". Whether this setting is used depends on the respective PDF reader.
                "autoprint": This flag specifies the conforming reader to automatically display a
                print dialog when the document is opened. "hidetoolbar": This flag specifies whether
                to hide the conforming reader's tool bars when the document is active. The default
                value is "false". "hidemenubar": This flag specifies whether to hide the conforming
                reader's menu bar when the document is active. The default value is "false".
                "hidewindowui": This flag specifies whether to hide user interface elements in the
                document's window (such as scroll bars and navigation controls), leaving only the
                documents's contents displayed. The default valie is "false". "fitwindow: This flag
                specifies whether to resize the document's window to fit the size of the first
                displayed page. The default value is "false". "centerwindow": This flag specifies
                whehter to position the document's window in the center of the screen. The default
                value is "false". "displaydoctitle": This flag specifies whether the window's title
                bar should display the document title taken form the Title entry of the document
                information dictionary. If "false", the title bar should instead display the name of
                the PDF file containing the document. The default value is "false". "printscaling":
                Specifies which option will be selected automatically whwn the Print dialog will be
                displayed. Possible values are "APPDEFAULT" and "NONE". -->

      <viewer pagemode="None" autoprint="false" hidetoolbar="false" hidemenubar="false" hidewindowui="false" fitwindow="false" centerwindow="false" displaydoctitle="false" printscaling="AppDefault"/>

      <!--Specification of the Type3 font resolution in dpi. The default
                value is 300dpi. For output bitmap TYPE3 fonts use the value 0; For for input vector
                fonts use the default setting 300 dpi; Otherwise, use the font resolution of the
                input bitmap or the "independent fontresolution".-->

      <fontresolution value="0"/>

      <!--The attribute "standard" of element "formfields" specifies 
                whether the PDF reader should construct appearences for form fields (TRUE) 
                or not (FALSE). This corresponds directly to the adding "NeedAppearances" entry 
                to the output. "NeedAppearences" is depreciated in version 2.0 of the 
                PDF specification.-->

      <formfields standard="false"/>

      <imagecompressions>
         <!--Controls the image compression type. Invalid settings are ignored. Multiple
                "imagecompression" elements are allowed (basically one for every possible
                (bitsperpixel, colorformat) pairing. Valid values for bits per pixel: "1", "2", "4",
                "8", "24", "32" Valid values for colorformat: GRAYSCALE, PALETTE, RGB, CMYK
                Monochrome equals 1 bit per pixel grayscale. Valid values for compression are:
                DEFAULT, FAXG4, JPEG, ZIP. Incompatible or not supported compressions are ignored.
                The attribute "binarycopy" specifies whether whether the images from the input
                document are to be copied (TRUE) or whether they are first decompressed and then
                compressed anew (FALSE). Binary copy is faster but the image is not validated. This
                may produce useless PDFs, at least when used with some PDF readers. If PDF/A is set
                as target format, binary-copy is disabled, regardless the setting for this
                attribute. The compression used for pattern images can be specified with the 
                "pattern-compression" attribute, valid values are : DEFAULT, FAXG4, JPEG, ZIP.-->

         <imagecompression bitsperpixel="1" colorformat="GrayScale" compression="Default" binarycopy="false" pattern-compression="Default"/>
         <imagecompression bitsperpixel="8" colorformat="GrayScale" compression="Default" binarycopy="false"/>
         <imagecompression bitsperpixel="24" colorformat="Rgb" compression="Default" binarycopy="false"/>
      </imagecompressions>

      <!--Specifies the quality of written JPEG images as percentage. Valid values
                range from 0 to 100%. The default setting is 75%.-->

      <jpegquality value="75"/>

      <!--Specifies how fonts are handled for the creation of output documents. Valid
                values of the attribute "default" are: 
                * AUTO, - Automatic selection of one of the following settings with respect to conversion 
                exactness and restrictions concerning the font availability. 
                * RASTERIZE, - maintains original text aspect, but text is replaced with images. No 
                alignment problems or missing characters but filesize is bigger and text not searcheable. 
                Adobe-documented compatibilty problems with some HP-printers Use "ignoreimagemasks" to 
                avoid them (text background loses transparency) Used automatically when missing characters 
                are detected in the selected font and raster information is available.
                * CONVERTTOTYPE3, - generate Type3 raster with the same appearence and characters as original 
                font. Makes text searcheable. Filesize generally smaller than with RASTERIZE No alignment 
                problems or missing characters. Adobe-documented compatibilty problems with some HP-printers 
                "ignoreimagemasks" solution does not function in this case (Acrobat does not accept 
                non-transparent type3 glyphs). Never chosen automatically.
                * USESTANDARDFONTS - uses only the 14 Type1 Adobe standard fonts. Smallest file size. May 
                result in text alignment problems and/or missing characters (if no raster information is 
                present in input). No compatibility problems. Chosen automatically only if the input file 
                font does not contain font-metric information.
                * USEBESTMATCHINGFONTS - force Acrobat to use MultipleMaster fonts with exactly the same metrics 
                as the original font -> This may prevent text alignment problems, but problems due to lacking 
                characters may remain. Deactivate the embedding of original fonts to reduce the file size of the
                PDF output file. Adobe-documented compatibilty problems with some HP-printers. No workaround, 
                except for disabling it. Chosen automatically if original font (in compatible format) is not 
                available and input font metrics are known.
                * USEORIGINALFONTS - Use the original font data (when it is TrueType or Type1) from input file 
                and embed it. No alignment or missing characters problems. No compatibility problems. Preferred 
                over other settings in AUTO-mode. No really need to explicitly specify it, present for sake of 
                completeness.
                "split-optimization": Specifies whether Type3 fonts are created
                only once to produce several output documents ("true") or not ("false"). This may
                increase speed when several output documents are created from one input.
                "autotype3": If set to "true", all fonts that are not TrueType or Type1 fonts and
                can be rasterized are converted to Type3 fonts provided that "default" is set to
                AUTO or USEORIGINALFONTS. It will not affect RASTERIZE, USESTANDARDFONTS
                or USEBESTMATCHINGFONTS. "intensify": whereby they appear slightly bolder. If set to
                true, texts with Type3 fonts are rendered using the setting "fillandstroke". This
                will make the text to look minimal bolder. Use this setting to display text darker
                even at lower resolutions, e.g. in full page mode. This setting is used to achieve a
                better display in certain versions of Adobe Acrobat 9.x. It can lead to unexpected
                effects in other versions or when other readers are used.-->
      <!--Attribute: mapBoundedFont @comment:  By default, bounded fonts can be mapped in the fontlist. This attribute is used
                to activate or deactivate the mapping of bounded fonts. -->

      <fonthandling default="Auto" split-optimization="false" autotype3="true" mapBoundedFont="true"/>

      <!--Sets split-optimization to true when multiple output files are generated from
            1 input file (feature of -splitmask and -splitdelta or Toolkit) in order to avoid that the
            same image resources are compressed multiple times. If this setting is used in other scenarios, 
            the memory consumption may be affected in a negative way, i.e. memory consumption may increase.-->

      <ImageHandling split-optimization="false"/>

      <!--Specification whether archive fields (AFP: TLEs or index values) or PDF
                comments (ANNOTATIONS) are to be written. Valid values are NOT, ANNOTATIONS,
                PIECEINFO.-->

      <writearchivefields value="Not" invisible="false" application="Compart"/>
   </output>

   <resourcelist>
      <fileslist>
         <files path="/opt/Compart/res/icc" type="ICC" extension="icc" />
         <files path="/opt/Compart/res/pfb" type="Type1Font" extension="pfb" />
         <files path="/opt/Compart/res/ttf" type="TrueType" extension="ttf" />
         <files path="E:/Compart/res/icc" type="ICC" extension="icc" />
         <files path="E:/Compart/res/pfb" type="Type1Font" extension="pfb" />
         <files path="E:/Compart/res/ttf" type="TrueType" extension="ttf" />
         <files path="c:\WINDOWS\Fonts" type="TrueType" extension="ttf" />
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/icc" type="ICC" extension="icc" />
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/pfb" type="Type1Font" extension="pfb" />
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/ttf" type="TrueType" extension="ttf" />					

      </fileslist>
   </resourcelist>
</mffpdf>
