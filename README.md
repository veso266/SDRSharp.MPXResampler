# SDRSharp.MPXResampler
This is just a small demonstration plugin, which shows you how to transfer audio between SDR# and the soundcard and also shows you how to resample it (eather using a class or almost by hand)
 
# How to install
Download [dll](https://github.com/veso266/SDRSharp.MPXResampler/releases/download/1.0/SDRSharp.MPXResampler.dll) from release page
## For latest SDR#
You just copy dll into Plugins directory and enjoy
## For SDR# 1785 and below 
You copy the plugin DLL into sdrsharp program directory and then edit plugins.xml and add the following line to it (people used to call this  MagicLine.txt)
 ```xml
<add key="MPXResampler" value="SDRSharp.MPXResampler.MPXResamplerPlugin,SDRSharp.MPXResampler" />
 ```
 
 Exmplanation of MagicLine(if you want to write your own plugin someday)
 ```xml
 <add key="Whatever-you-want-that-is-unique" value="NameSpace.EntryPoint,DLLName" />
 ```
