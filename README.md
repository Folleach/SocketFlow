# SocketFlow
An event-oriented protocol over tcp  
_Light_ and _simple_ for use

#### Supported languages
|      | client | server |
|:-----|:------:|:------:|
| C#   | +      | +      |
| Js   | Wip    | -      |
| Java | -      | -      |

### Protocol
* Have a simple overhead (8 bytes for every event)
* Ability to transfer 2 GB of event (2147483639 bytes)
* Ability to use your data structure (Named **DataWrapper**)  
  such as:
  - Json
  - Xml
  - Raw bytes
  - Your own structure
* May be created 2 billion different events

Every event looks as:
```
      ┌───────┬───────┬───────────────┐ 
      │ type  │length │ event data... │
      ├─┬─┬─┬─┼─┬─┬─┬─┼─┬─┬─┬─┬─┬─┬─┬─┤  
bytes └─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┴─┘
       0 1 2 3 4 5 6 7 8 9 ...
```
First 4 bytes (int) it is type of event, determines which event occurred.  
Second 4 bytes (int) it is length of data, no more.  
Then followed by _data_, which will be converted to _value_ using the **data wrapper**.
