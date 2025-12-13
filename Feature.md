# Finished-Feature

+ ### `BinaryStreamToolkit`
    + 用于以泛型的方式读写Stream中的数据
        + 主要行为: Ex:Stream
        + 命名空间: Iks.BinaryToolkit.BinaryStreamToolkit
        + 源码位置: 自己翻
        + `ReadAs<T>`(out与返回值模式两种出参方式)、`WriteFrom<T>`
+ ### `Multiple-BinaryStreamToolkit`
    + 用于以泛型的方式读取多个数据,提供`指针(T*)`与`Span`支持
        + 主要行为: Ex:Stream
        + 命名空间: Iks.BinaryToolkit.BinaryStreamToolkit
        + 源码位置: 自己翻
        + `ReadManyAs<T>`、`WriteManyFrom<T>`
+ ### `EndianToolkit`
    + 用于以泛型的方式反转端序，算是`BinaryPrimitives`的增加与封装
        + 主要行为: static: EndianToolkit
        + 命名空间: Iks.BinaryToolkit.EndianToolkit
        + 源码位置: 自己翻
        + `Convert<T>(value,from,to)`语义明确，支持填写Local: `from Local to Big`
        + 能`allows ref struct`的尽力allow了，如果`ref struct`的内存布局里真有`指针(T*,ref)`不行(
          *`.NET 8因为不能allow也不行`*)
        + `Reverse<T>` 同样的`指针(T*)`与`Span`出参
+ ### Multiple-EndianToolkit
    + 用于以泛型的方式反转集合元素端序，与`Multiple-BinaryStreamToolkit`配合使用
        + 主要行为: static: EndianToolkit
        + 命名空间: Iks.BinaryToolkit.EndianToolkit
        + 源码位置: 自己翻
        + `ConvertMany<T>(values,from,to)`同上，针对多个元素不会多次评估是否包含`指针(T*,ref)`
        + `ReverseMany<T>`依旧是`Span`和`指针(T*)`
+ ### `Pin<T>` ,A `GCHandle` Wrapper
    + 为NET10以下版本提供一个带Dispose方法的GCHandle
    +
    NET10以上请使用标准库[PinnedGCHandle\<T>](https://learn.microsoft.com/dotnet/api/system.runtime.interopservices.pinnedgchandle-1?view=net-10.0)
        + 行为与NET10的`PinnedGCHandle`有些不一致,`PinnedGCHandle`类型范围更大,它不检查对象是否是`可Pin`的
        + 我的实现`Pin`作为`GCHandle`的封装,并不能绕过`GCHandle`的API限制
        + 主要行为: struct Pin<TPinned>/static PinExtension
        + 命名空间: Iks.BinaryToolkit.UnsafeToolkit.Pin<TPinned>
        + 源码位置: 自己翻
    + 用途: 暂时pin住一个对象，用于与非托管代码交互，主要提供语法简洁性、额外的null检查、using自动释放

> [!CAUTION]  
> Remember To Dispose It!

# Thinking

+ ## 还没想好(大声)
+ ### 欢迎贡献
    