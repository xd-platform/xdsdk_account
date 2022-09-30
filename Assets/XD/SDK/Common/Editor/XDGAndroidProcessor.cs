#if UNITY_EDITOR && UNITY_ANDROID
using System.IO;
using System.Text;
using LC.Newtonsoft.Json;
using UnityEditor.Android;
using UnityEngine;
using XD.SDK.Common.Editor;

public class XDGAndroidProcessor : IPostGenerateGradleAndroidProject{
    
    private StringBuilder launchStr = new StringBuilder();
    private StringBuilder baseStr = new StringBuilder();
    private StringBuilder implStr = new StringBuilder();

    void IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path){
        var projectPath = path;
        if (path.Contains("unityLibrary")){
            projectPath = path.Substring(0, path.Length - 12);
        }

        //拷贝 google-services 可选
        var parentFolder = Directory.GetParent(Application.dataPath)?.FullName;
        var googleJsonPath = parentFolder + "/Assets/Plugins/Android/google-services.json";
        if (File.Exists(googleJsonPath))
        {
            Debug.Log("拷贝谷歌 google-services");
#if UNITY_2019_1_OR_NEWER
            File.Copy(googleJsonPath, projectPath + "/launcher/google-services.json",true);
            File.Copy(googleJsonPath, projectPath + "/unityLibrary/src/main/assets/google-services.json", true);
#else
            File.Copy(googleJsonPath, projectPath + "/google-services.json",true);
            File.Copy(googleJsonPath, projectPath + "/src/main/assets/google-services.json", true);
#endif

            //动态配置Firebase信息
            launchStr.Append(@"
apply plugin: 'com.google.gms.google-services'
apply plugin: 'com.google.firebase.crashlytics'
            ");

#if UNITY_2019_1_OR_NEWER
            baseStr.Append(@"allprojects {
    buildscript {
        dependencies {
            classpath 'com.google.gms:google-services:4.0.2'
            classpath 'com.google.firebase:firebase-crashlytics-gradle:2.2.1'
        }
    }
}");
#else
            baseStr.Append(@"
    classpath 'com.google.gms:google-services:4.0.2'
    classpath 'com.google.firebase:firebase-crashlytics-gradle:2.2.1'
");
#endif
            implStr.Append(@"
    implementation 'com.google.firebase:firebase-core:18.0.0'
    implementation 'com.google.firebase:firebase-messaging:21.1.0'
    implementation 'com.google.android.gms:play-services-auth:16.0.1'
    implementation 'com.google.android.gms:play-services-ads-identifier:15.0.1'

           ");
            
        } else{
            implStr.Append(@"
    implementation 'com.google.android.gms:play-services-auth:16.0.1'
           ");
            Debug.LogError("打包失败 ---  googleJsonPath 不存在");
        }

        processXDConfig(); //动态配置安卓库

        //配置路径
        var gradlePropertiesFile = projectPath + "/gradle.properties";
        var baseProjectGradle = projectPath + "/build.gradle";
#if UNITY_2019_1_OR_NEWER
        var launcherGradle = projectPath + "/launcher/build.gradle";
        var unityLibraryGradle = projectPath + "/unityLibrary/build.gradle";
#else
        var launcherGradle = baseProjectGradle;
        var unityLibraryGradle = baseProjectGradle;
#endif
        
#if UNITY_2019_1_OR_NEWER
        //apply plugin 
        if (File.Exists(launcherGradle)){
            Debug.Log("编辑 launcherGradle");
            var writerHelper = new XD.SDK.Common.Editor.XDGScriptHandlerProcessor(launcherGradle);
            writerHelper.WriteBelow(@"apply plugin: 'com.android.application'", launchStr.ToString());
        }
#else
        if (File.Exists(launcherGradle)){
            Debug.Log("编辑 launcherGradle");
            var writerHelper = new XD.SDK.Common.Editor.XDGScriptHandlerProcessor(launcherGradle);
            writerHelper.WriteBelow(@"apply plugin: 'com.android.application'", launchStr.ToString());
        }
        else
        {
            Debug.LogError("打包失败 ---  launcherGradle 不存在");
        }
#endif

#if UNITY_2019_1_OR_NEWER
        //classpath 
        if (File.Exists(baseProjectGradle)){
            Debug.Log("编辑 baseProjectGradle");
            var writerHelper = new XD.SDK.Common.Editor.XDGScriptHandlerProcessor(baseProjectGradle);
            writerHelper.WriteBelow(@"task clean(type: Delete) {
    delete rootProject.buildDir
}", baseStr.ToString());
        }
#else
        //classpath 
        if (File.Exists(baseProjectGradle)){
            Debug.Log("编辑 baseProjectGradle");
            var writerHelper = new XD.SDK.Common.Editor.XDGScriptHandlerProcessor(baseProjectGradle);
            writerHelper.WriteBelow(@"repositories {
        mavenCentral()
        google()
        jcenter()
    }

    dependencies {", baseStr.ToString());

        }
#endif
        else
        {
            Debug.LogError("打包失败 ---  baseProjectGradle 不存在");
        }

        //implementation 
        if (File.Exists(unityLibraryGradle))
        {
            Debug.Log("编辑 unityLibraryGradle");
            var writerHelper = new XD.SDK.Common.Editor.XDGScriptHandlerProcessor(unityLibraryGradle);
            writerHelper.WriteBelow(@"implementation fileTree(dir: 'libs', include: ['*.jar'])", implStr.ToString());
        }
        else
        {
            Debug.LogError("打包失败 ---  unityLibraryGradle 不存在");
        }

        processUnityVersionChange(projectPath);
    }

    private void processXDConfig(){
        var parentFolder = Directory.GetParent(Application.dataPath)?.FullName;
        var jsonPath = parentFolder + "/Assets/Plugins/XDConfig.json";
        if (!File.Exists(jsonPath)){
            Debug.LogError("/Assets/Plugins/XDConfig.json 配置文件不存在！");
            return;
        }
        

        var configMd = JsonConvert.DeserializeObject<XDConfigModel>(File.ReadAllText(jsonPath));
        if (configMd == null){
            Debug.LogError("/Assets/Plugins/XDConfig.json 解析失败！");
            return;
        }

        //配置第三方库
        if (configMd.facebook != null && !string.IsNullOrEmpty(configMd.facebook.app_id)){
            implStr.Append(@"
    implementation 'com.facebook.android:facebook-login:12.0.0'
    implementation 'com.facebook.android:facebook-share:12.0.0'

           ");
        }
        
        if (configMd.twitter != null && !string.IsNullOrEmpty(configMd.twitter.consumer_key)){
            implStr.Append(@"
    implementation 'com.twitter.sdk.android:twitter:3.3.0'
    implementation 'com.twitter.sdk.android:tweet-composer:3.3.0'

           ");
        }
        
        if (configMd.appsflyer != null && !string.IsNullOrEmpty(configMd.appsflyer.dev_key)){
            implStr.Append(@"
    implementation 'com.appsflyer:af-android-sdk:6.5.2'
    implementation 'com.appsflyer:unity-wrapper:6.5.2'

           ");
        }
        
        if (configMd.adjust != null && !string.IsNullOrEmpty(configMd.adjust.app_token)){
            implStr.Append(@"
        implementation 'com.adjust.sdk:adjust-android:4.24.1'

           ");
        }
        
        if (configMd.line != null && !string.IsNullOrEmpty(configMd.line.channel_id)){
            implStr.Append(@"
    implementation 'com.linecorp:linesdk:5.0.1'

           ");
        }
        
        Debug.Log($"配置 launchStr: \n{launchStr}");
        Debug.Log($"配置 baseStr: \n{baseStr}");
        Debug.Log($"配置 implStr: \n{implStr}");
    }

    /// <summary>
    /// 处理因为 Unity 版本不同导致需要做的工作
    /// </summary>
    private void processUnityVersionChange(string projectPath)
    {
#if !UNITY_2020_3_OR_NEWER
        var baseProjectGradle = projectPath + "/build.gradle";
        var writerHelper = new XDGScriptHandlerProcessor(baseProjectGradle);
        // 升级 Android Gradle Plugin 版本
        writerHelper.Replace(@"classpath 'com.android.tools.build:gradle:3.4.0'", 
            @"classpath 'com.android.tools.build:gradle:4.0.1'");
#endif
        
    }
    public int callbackOrder{
        get{ return 999; }
    }
}
#endif