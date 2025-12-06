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
    + 能`allows ref struct`的尽力allow了，如果`ref struct`的内存布局里真有`指针(T*,ref)`不行(*`.NET 8因为不能allow也不行`*)
    + `Reverse<T>` 同样的`指针(T*)`与`Span`出参
+ ### Multiple-EndianToolkit
  + 用于以泛型的方式反转集合元素端序，与`Multiple-BinaryStreamToolkit`配合使用
    + 主要行为: static: EndianToolkit
    + 命名空间: Iks.BinaryToolkit.EndianToolkit
    + 源码位置: 自己翻 
    + `ConvertMany<T>(values,from,to)`同上，针对多个元素不会多次评估是否包含`指针(T*,ref)`
    + `ReverseMany<T>`依旧是`Span`和`指针(T*)`

# Thinking
+ ## 还没想好(大声)
+ ### 欢迎贡献
    