<?xml version="1.0" encoding="UTF-8"?>

<mffxff xmlns="http://www.compart.com/ns/mff/xff" version="201909">
   <!--
      **************************************************************************

      Copyright (C) Compart AG  2011

      Release: 201909

      **************************************************************************
-->
   <!--Font specified by "family", "width" and "style" can be rasterised To
                rasterize the text, rasterize="always" has to be specified. Important: for all point
                sizes correct or dummy device names must be specified -->

   <fontlist>
       <font family="Arial">
         <face weight="Medium" style="Upright"  fontfile="_a______" fontfiletype="Type1" />
         <face weight="Medium" style="Italic" fontfile="_ai_____" fontfiletype="Type1" />
         <face weight="Bold" style="Upright" fontfile="_ab_____" fontfiletype="Type1" />
         <face weight="Bold" style="Italic" fontfile="_abi____" fontfiletype="Type1" />
      </font>
       <font family="Arial,Bold" >
         <face weight="Bold" style="Upright" fontfile="_ab_____" fontfiletype="Type1" />
      </font>

	  <font family="ArialMT" >        
         <face weight="Medium" style="Upright"  fontfile="_a______" fontfiletype="Type1" />
         <face weight="Medium" style="Italic"  fontfile="_ai_____" fontfiletype="Type1" />
         <face weight="Bold" style="Upright" fontfile="_ab_____" fontfiletype="Type1" />
         <face weight="Bold" style="Italic" fontfile="_abi____" fontfiletype="Type1" />
      </font>

	  <font family="Helvetica" >
         <face weight="Medium" style="Upright"  fontfile="_a______" fontfiletype="Type1" />
      </font>

	  <font family="Helvetica-Bold" >
         <face weight="Bold" style="Upright" fontfile="_ab_____" fontfiletype="Type1" />
      </font>

   </fontlist>

   <!--Settings for input.-->

   <input>
      <!--true : TLEs are being read (default) false : TLEs are ignored (faster),
                only used for TLEs in XFF files created before release 201202-->

      <readarchivefields value="true"/>

      <!--true : NOPs are being read (default) false : NOPs are ignored (faster),
                only used for NOPs in reading XFF files created before release 201202-->
      <!--Attribute: value @comment: TRUE: NOPs are read. FALSE: NOPs are ignored (improves speed).
            -->

      <readcomments value="true"/>
   </input>

   <!--Settings for output.-->

   <output>
      <!--Resolve references to other pages (overlays, ...).-->
      <measurementunits value="1440" default="1440"/>
      <resolveoverlays value="false"/>

      <!--The optimization of overlapping graphics
                objects.-->

      <optimizepa value="never" colorSpaces="GRAY,CMYK"/>

      <optimizepaoptions>
         <ignoreitems value="EXTERNAL"/>

         <!--Using optimizepa, the underline of a text can be lost. Enabled
                separateunderlinesfromtext with active optimizepa separates the underline from the
                text item. By default this switch is disabled.-->

         <separateunderlinesfromtext value="false"/>

         <!--Optimization renders items to images. By default images are created with
                the bottom-up orientation. But, if top-down orientation of the created imagse is
                expected, this setting should be set to the topDown. -->

         <ImageOrientation value="bottomUp"/>
      </optimizepaoptions>

      <imagecompressions>
         <!--Control image compression type. Incompatible types are ignored Multiple
                xml-imagecompression-elements are allowed (basically one for every possible
                (bitsperpixel, colorformat) pairing. Valid values for bits per pixel: "1", "4", "8",
                "24", "32" Valid values for colorformat: "grayscale", "palette", "rgb", "cmyk"
                Monochrome is 1 bitperpixel grayscale. Valid values for compression are: "default",
                "none", "rle", "lzw", "packbits", "ccittrle", "faxg3", "faxg4", "mmr", "ipfix",
                "jpeg", "pcd", "jpegnew", "zip", "jp2", "jbig2" Incompatible or not supported
                compressions are ignored-->

         <imagecompression bitsperpixel="1" colorformat="grayscale" compression="faxg4" keeporiginal="false" separated="false"/>
         <imagecompression bitsperpixel="8" colorformat="grayscale" compression="default" keeporiginal="true"/>
         <!--
         <imagecompression bitsperpixel="24" colorformat="rgb" compression="jpeg" keeporiginal="true"/>
         -->
         <imagecompression bitsperpixel="32" colorformat="cmyk" compression="lzw" keeporiginal="false" separated="true"/>
      </imagecompressions>

      <!--Specifies the quality of written JPEG images as percentage. Valid values
                range from 0 to 100%. The default setting is 75%.-->

      <jpegquality value="75"/>

      <!--Compression level for ZIP compression 0..9, 0=none(fast), 9=max (slow, best
                compression)-->

      <zipcompression level="9"/>

      <!--Resolution (DPI) to use for generated raster fonts. If the input font
                already has a resolution, it will be used, otherwise the specified value here will
                be used. Default 0: let the application take the decision.-->

      <fontresolution value="0"/>

      <!--Controls the handling of fonts for output file creation.
                "default": Default font handling, this does not affect raster/bitmap-fonts! auto -
                This is the default. Currently the same as useOriginalFonts. useOriginalFonts - Use
                / embed the original font data. keepMetricsOnly - maintains the metrics, but drops
                all other font data. "writeFallbackFonts": Specifies if fallback fonts are written
                in the output file or if the glyphs used from this font will be rastered. Default
                value is true. -->

      <fonthandling default="auto" writeFallbackFonts="true"/>

      <!--To support a file size of more than 4 GB, set the value to 8.
                Recommendation is to use 8. Default is 8 (values 4 or 8)-->

      <chunksizelength value="8"/>
   </output>

   <!--The colorprofilelist element contains one colorprofile element per color space. 
            An ICC profile is a standardized record that defines the color space of a device. 
            The ICC profiles can either have the file extension icc (Windows) or icm (Mac).
            In addition to specifying the file name of the color profile, you can use the resourcelist element 
            to specify which directories are to be searched for the specified color profiles.
            -->
   <!--<colorprofilelist>        
      <colorprofile name="GenericRGB"/>        
      <colorprofile name="GenericCMYK"/>        
      <colorprofile name="GenericGray"/>
   </colorprofilelist>-->

   <resourcelist>
      <fileslist>
         <files path="E:/Compart/res/icc" type="ICC" extension="icc"/>
         <files path="E:/Compart/res/pfb" type="Type1Font" extension="pfb"/>
         <files path="E:/Compart/res/ttf" type="TrueType" extension="ttf"/>
         <files path="c:\WINDOWS\Fonts" type="TrueType" extension="ttf"/>
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/icc" type="ICC" extension="icc" />
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/pfb" type="Type1Font" extension="pfb" />
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/ttf" type="TrueType" extension="ttf" />

                  <!--<files path="\resource" type="icc" extension="icc"/>-->
         <!--<files path="\resource" type="icc" extension="icm"/>-->
      </fileslist>
   </resourcelist>
</mffxff>
