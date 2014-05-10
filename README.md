ResX-Aggregator
===============

Easy the resource(.resx) file editing. 

**Feature**

Aggregate all the .resx files into one editor.

If you have these .resx files under a folder:
- index.cshtml.resx
- index.cshtml.en-US.resx
- index.cshtml.zh-CN.resx

It's annoying that edit them one by one. Why don't edit them all in a single file?

Now with ResXAggregator, you can add a file that named "index.cshtml.resx-aggregator", and then open it in visual studio, it will allow you edit all the different cultures and save them back in just one single editor. It displays all the resource settings as this way:

	| Name            | default                  | en-US                | zh-CN       |
	| CloseLinkDialog | Stäng                    | Close                | 关闭         |
	| CloseMsg        | Stäng meddelandefönstret | Close message window | 关闭消息窗口 |
	| CloseTips       | Stäng hjälp              | Close help           | 关闭帮助     |

**Screenshot**

![Screenshot for ResXAggregator's UI](ResX-Aggregator/Templates/ResXAggregator.png "Screenshot for ResXAggregator's UI")

![Use ResX-Aggregator in BlogEngine.NET project](ResX-Aggregator/Templates/Use ResXAggregator in BlogEngine.NET.png "Use ResX-Aggregator in BlogEngine.NET project")

**Release Notes**

V1.3	2014-2-14
- Fixed ["Can't copy the text inside the cells #3"](https://github.com/Jeff-Tian/ResX-Aggregator/issues/3)
- Fixed ["When trying to delete an item from the grid editor, it fails with error: The collection is readonly. #1"](https://github.com/Jeff-Tian/ResX-Aggregator/issues/1)

**Develop**

You need to run ~/Tools/vssdk_full(2013).exe to install visual studio SDK to open the VSIX project in Visual Studio 2013.