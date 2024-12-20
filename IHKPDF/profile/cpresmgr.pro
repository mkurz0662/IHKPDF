<?xml version="1.0" encoding="UTF-8"?>

<cpresmgr xmlns="http://www.compart.com/ns/mff/cpresmgr" version="201207">
   <!--
      **************************************************************************

      Copyright (C) Compart AG  2011

      Release: 201207

      **************************************************************************
-->

   <globals>
      <!--
                backup="true"   means first create a backup of old xml file
                and then overwrite it with new format.

                note:  The backup option is only useful when updating old xml
                file into new format. The rule will not apply with
                new format.

                action="enable|disable"
                User can enable or disable font database using this option
            -->

      <database name="font" filename="ftmgr.xml" backup="true" enable="true">
         <!--
                Option "save" is used to store the database information in given xml file
                when application is terminated.

                by default this value is "false"
            -->

         <option name="save" value="false" action="none"/>

         <!--
                option "scan" can be defined with different combination
                Possible scanning options are
                on-create :     scan fonts during the resource manager creation
                on-missing-db:  scan fonts when database file is missing
                on-query:       scan fonts when font not found from given paths
                force-rescan:   forcefully rescan paths
                ignore-db:      ignore database file if exist

                default value of option scan are "on-missing-db"

                Similiarly user can perform different actions after the scaning
                Possible actions are "none" and "create"
                "none"          means perform nothing
                "create"        means create database after the scan

                At a moment "create" action is only used with on-missing-db,
                which means create xml database file if not exist.

                values           :      default actions
                on-missing-db               create
                on-create                   none
                force-rescan                none
                on-query,                   none
                ignore-db                   none
                monotype-fonts              disable|enable
            -->

         <option name="scan" value="on-missing-db" action="create"/>
         <option name="scan" value="monotype-fonts" action="disable"/>

         <!--
                The Resource Search order can be changed on based user requirement
                The default search order is volatile, user, adobe ,system
                User can change this order according to his requirement
                the alternate name of element <scan> is <searchorder>.
                Both have same affect
            -->

         <scan>
            <path name="volatile" matchmode="exact"/>
            <path name="user" matchmode="exact"/>
            <path name="adobe" matchmode="exact"/>
            <path name="system" matchmode="standard"/>
            <path name="monotypefonts" matchmode="exact"/>
         </scan>
      </database>
   </globals>

   <!--
                type can be "TrueType", "Type1Font", "Type1Metrics",
                if it is not defined, the path will be used for all types
                Extension can be emtpy, extension or wildcard
            -->

   <resourcelist>
      <fileslist>
         <files path="E:/Compart/res/pfb" type="Type1Font" extension="pfb"/>
         <files path="E:/Compart/res/ttf" type="TrueType" extension="ttf"/>
         <files path="c:/windows/fonts" type="TrueType" extension="ttf"/>
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/icc" type="ICC" extension="icc" />
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/pfb" type="Type1Font" extension="pfb" />
         <files path="//d192s008/produktion/Programme/Compart/CompartToolkit/res/ttf" type="TrueType" extension="ttf" />
      </fileslist>
   </resourcelist>
</cpresmgr>
