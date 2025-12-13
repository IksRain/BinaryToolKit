# Feature:

#### [跳转到`Feature`](Feature.md)

# Github-Actions

#### 可能我调的`actions`比我的库有用一点(

#### [跳转到我的`Actions`](.github/workflows/dotnet-build-publish.yml)

+ 只需要为仓库添加`NUGET_API_KEY机密`并修改`env: project`即可使用
+ **通过`打Tag`执行发布流程**
    + `版本号` 就是 `Tag`名
    + `Preview版本`在`Tag`中后缀`preview`
    + 自动生成符号包`snupkg`并推送
    + 您只需要关心`Icon`,`LICENSE`,`ReadMeFile`和`包描述`即可(都是IDE中很好指定的)

# 为什么我们需要泛型支持？

+
    + `T Stream.ReadAs<T>()`
        + 用于从Stream中读取一个非托管类型数据(如int,Int128)
    + `void Stream.Write<T>(in T)`
        + 用于向Stream中写入一个非托管类型数据(如int,Int128)
    + ### 我们不需要将函数名当泛型使用的函数,请考虑以下场景
    + 如果您需要实现一种二进制格式的数据的读写，如果您使用NET提供的蹩脚`BinaryReader`作为您的辅助
```csharp
class DataReader
{
  BinaryReader _reader;
  public int ReadInt(){
      return _reader.ReadInt();
  }
  ...
}
```
您会发现您被BinaryReader绑架了，要么你自己也各种`ReadInt`,`ReadShort`，要么你就得switch进行类型分发，现在，我替您完成了简单的类型读写操作


+ `Iks.Binary.EndianToolkit`理由同上，且有更严重的类型分发问题
    + 如果您采用了泛型，即使您将泛型参数类型限制到`INumber<TSelf>`，您也用不了`BinaryPrimitives`
      进行读写，如果您不希望使用非安全代码您可能会写出类似下面的东西
  ```csharp
    void Usage<T>(T value)
    {
      switch (value)
      {
         case int i:
           ... 
         case short s:
           ...
         case long l:
           ...
      }
     }
    ```
  现在，我也替您完成了泛型的适配，尽管使用非安全代码可以很快完成这些操作，但我由衷希望您能省略这些样板代码

    
  