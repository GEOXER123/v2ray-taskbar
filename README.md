## 说明
v2ray-taskbar 启动时会从 v2ray-taskbar.ini 文件中读取 V2Ray 可执行文件和配置文件，用于
后台启动 V2Ray 程序。路径格式支持绝对路径和相对路径，填相对路径时，路径起点为 v2ray-taskbar 
可执行文件所在的目录。

## 配置文件示例
v2ray-taskbar.ini
> [config]
> ;本ini文件必须与v2ray-taskbar文件同名且在同一目录中
> ;v2ray启动参数中文件可以写绝对路径也可以写相对路径
> 
> ;指定 V2Ray 可执行文件。留空时使用默认值：同目录下的v2ray.exe文件
> v2ray_exe="C:\MyProgram\v2ray-x64\v2ray.exe"
> 
> ;指定 V2Ray 配置文件，文件格式为json。留空时使用默认值：同目录下的config.json文件
> v2ray_conf=".\config.json"

## License

GPLv3
