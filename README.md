Timer
=========

### Actionscript like Timer class

## Usage
```csharp
public Int32 counter = 0;
public Timer timer = new Timer(1000, 10);

public void countNum(object sender, EventArgs e) { counter++; Console.Write(counter); }
public void writeNum(object sender, EventArgs e) { Console.Write("End"); }

timer.AddEventListener(TimerEvent.TIMER, countNum);
timer.AddEventListener(TimerEvent.TIMER_COMPLETE, writeNum);
timer.Start();

// 1
// 2
// 3
// ...
// 10
// End
```

## LICENSE

[MIT](http://www.opensource.org/licenses/mit-license.php)