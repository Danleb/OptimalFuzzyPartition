import os
import xml.etree.ElementTree as ET
from os import listdir
from os.path import isfile, join

ResFileNameStart = 'Resource.'
DesignerFileName = 'Resource.Designer.cs'
XamlResTitle = '<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"\n\
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"\n\
                    xmlns:v="clr-namespace:System;assembly=mscorlib">\n'
XamlResEnd = '</ResourceDictionary>'


def getFilesToConvert():
    files = os.listdir('.')

    filesToConvert = []

    for f in files:
        if f.startswith(ResFileNameStart) and not f == DesignerFileName:
            filesToConvert.append(f)

    return filesToConvert


def convertFile(resxPath):
    print('\nCurrent resx file: ' + resxPath)

    tree = ET.parse(resxPath)
    root = tree.getroot()

    outputXamlData = XamlResTitle

    for data in root.iter('data'):
        # print(data.attrib)
        key = data.get('name')

        value = data.find('value').text
        if value is None:
            value = 'NOT_LOCALIZED'

        #print(key + ' ' + value)

        outputXamlData += '<v:String x:Key="{}">{}</v:String>\n'.format(
            key, value)

        #print('String key: ' + key)
        #print('Value: ' + value)

    outputXamlData += XamlResEnd

    #startIndex = resxPath.index()
    # endIndex = resxPath.index('.resx')

    cultureCode = resxPath[len(ResFileNameStart): len(ResFileNameStart)+5]
    print(cultureCode)

    xamlFileName = 'StringLocalization.{}.xaml'.format(cultureCode)

    f = open(xamlFileName, 'w', encoding='utf-8')
    f.write(outputXamlData)
    f.close()


# main:
for f in getFilesToConvert():
    convertFile(f)
