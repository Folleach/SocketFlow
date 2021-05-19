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

<table>
    <thead>
        <tr>
            <th></th>
            <th colspan=4>length</th>
            <th colspan=4>type</th>
            <th colspan=6>event data</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>bytes:</td>
            <td>0</td>
            <td>1</td>
            <td>2</td>
            <td>3</td>
            <td>4</td>
            <td>5</td>
            <td>6</td>
            <td>7</td>
            <td>8</td>
            <td>9</td>
            <td>10</td>
            <td>11</td>
            <td>12</td>
            <td>...</td>
        </tr>
    </tbody>
</table>

First 4 bytes (int) it is length of data, no more.  
Second 4 bytes (int) it is type of event, determines which event occurred.  
Then followed by _data_, which will be converted to _value_ using the **data wrapper**.
