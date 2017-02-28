
-------------2017-1-20----------------- wanzhenyu@putao.com wulishan@putao.com
PTABManager_V2.0.1.unitypackage
1.加入projecttag 平台类游戏解决方案
2.加入上传多级目录的支持

-------------2016-12-13----------------- wanzhenyu@putao.com 
PTABManager_V1.0.9.unitypackage
1.label无法刷新临时解决方案
2.zip压缩文件去除.meta文件

-------------2016-12-05----------------- wanzhenyu@putao.com 
PTABManager_V1.0.8.unitypackage
1.添加文件夹压缩支持热更
2.添加文件热更
3.去除显示assetbunle 列表。通过三个标签来标记


-------------2016-11-23----------------- wanzhenyu@putao.com 
PTABManager_V1.0.7.unitypackage
1.修复上传ftp 文件改变大小问题。
2.去除上传.manifest文件到ftp
3.去除填写local path.改为选择是android,还是ios.


-------------2016-11-21----------------- wanzhenyu@putao.com 
PTABManager_V1.0.6.unitypackage
1.去除上传到online fpt 服务器功能
2.添加是否生成tpasset.cs类的选项
3.修改生成assetbunle的方式为ChunkBasedCompression，LZ4 format
4.添加lua 文件压缩为 data.zip文件
5.修改ptasset.cs可以支持assetbundle 二级目录.
6.コ?教ㄗ远?谢禾
8.去除调试用的删除 资源已更新 标志纪录
9.添加assetbundle 声称的ptassets.cs 中“@”符号替换为“_”
10.添加自动删除Assets同级缓存目录中非当前平台对应的文件夹及资源
11.添加forceclear用于清楚ssets同级缓存目录
12.修改自动上传ftp地址


-------------2016-09-12----------------- wanzhenyu@putao.com & liqngyun@putao.com
PTABManager_V1.0.5.unitypackage
1.editor 部分，奈猟ll

-------------2016-08-19----------------- wanzhenyu@putao.com & liqngyun@putao.com
PTABManager_V1.0.4.unitypackage
1.添加size,为md5

-------------2016-08-17----------------- wanzhenyu@putao.com & liqngyun@putao.com
PTABManager_V1.0.3.unitypackage
1.添加size,改为md5

-------------2016-08-16----------------- wanzhenyu@putao.com
PTABManager_V1.0.2.unitypackage
1.在编辑器中添加scrollview


-------------2016-08-02----------------- wanzhenyu@putao.com
PTABManager_V1.0.1.unitypackage
1.修复Android同步加载 streamingassets 肪渡柚律               if (Application.platform == RuntimePlatform.Android) {
					
					AssetBundleManager.SetSourceAssetBundleURL (Application.dataPath+"!assets" + "/AssetBundles/" + Utility.GetPlatformName () + "/");
				} else {
					AssetBundleManager.SetSourceAssetBundleURL (Application.streamingAssetsPath + "/AssetBundles/" + Utility.GetPlatformName () + "/");
				}


-------------2016-07-05----------------- wanzhenyu@putao.com
PTABManager_V1.0.0.unitypackage
1.基于unity 官方的 assetbundle manager 定制
2.主要针对ios版本
3.添加了右键标记需要打asset bundle的文件夹
4.改版本只能对文件夹进行打包
5.所有要打assetbundle的募?胁荒苤孛