/*

Name: Timer
Description: Actionscript like Timer Class.
Version: 0.0.1

*********************************************************************
	LICENSE
*********************************************************************
Copyright (c) 2014 Jun Ishibashi

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without 
restriction, including without limitation the rights to use, 
copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following 
conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.
 
*/

using System;
using System.Windows;
using System.Windows.Threading;

namespace Utils
{

public class Timer
{
	private DispatcherTimer _core;
	private EventHandler _tickEvent;
	private EventHandler _compEvent;
	private double _delay;
	private int _curCount;
	private int _repCount;
	private int _lastCount;
	private bool _tickInstanced;
	private bool _compInstanced;
	private bool _compReady;
    private Action<object, EventArgs> _tickListener;
    private Action<object, EventArgs> _compListener;
	private bool _isRunning;

	public Timer(double delay, int repeatCount = 0)
	{
		_core = new DispatcherTimer(DispatcherPriority.Normal);
		_delay = delay;
		_core.Interval = TimeSpan.FromMilliseconds(delay);
		_curCount = 0;
		_repCount = repeatCount;
		_tickInstanced = false;
		_compInstanced = false;
		_compReady = false;
		_isRunning = false;
	}
	//public ~Timer() { }
	
	public bool IsRunning { get { return _isRunning; } }
	public double Delay
	{
		get { return _delay; }
		set
		{
			if (_delay != value)
			{
				_core.Interval = TimeSpan.FromMilliseconds(value);
				_delay = value;
			}
		}
	}
	public int CurrentCount { get { return _curCount; } }
	public int RepeatCount { get { return _repCount; } }
	
	public void Reset() { Stop(); }
	
	public void Start() { _core.Start(); _isRunning = true; }
	
	public void Stop()
	{
		if (_isRunning)
		{
			_core.Stop();
			
			if (_compReady)
			{
				_core.Tick -= _compEvent;
				_core.Tick -= _lastFn;
			}
			
			_isRunning = false;
			_compReady = false;
			_curCount = 0;
		}
	}
	
	public void AddListener(string type, Action<object, EventArgs> listener)
	{
		if (type == TimerEvent.TIMER)
		{
			if (_tickInstanced) _core.Tick -= _tickEvent;
			
			_tickListener = listener;
			_tickEvent = new EventHandler(listener);
			_tickInstanced = true;
			_core.Tick += _tickEvent;
			
			if (_repCount != 0) _core.Tick += infCounter;
			else _core.Tick += limitCounter;
		}
		else if (type == TimerEvent.TIMER_COMPLETE)
		{
			if (_compInstanced) _core.Tick -= _compEvent;
		
			_compListener = listener;
			_compEvent = new EventHandler(listener);
			_compInstanced = true;
			_lastCount = _repCount - 1;
			_compReady = false;
		}
	}
	
	public void RemoveListener(string type, Action<object, EventArgs> listener)
	{
		if (type == TimerEvent.TIMER)
		{
			if (_tickInstanced)
			{
				if (_tickListener.Equals(listener))
				{
					if (_isRunning) Reset();
					
					_core.Tick -= _tickEvent;
					
					if (_repCount == 0) _core.Tick -= infCounter;
					else _core.Tick -= limitCounter;
					
					_tickInstanced = false;
				}
			}
		}
		else if (type == TimerEvent.TIMER_COMPLETE)
		{
			if (_compInstanced)
			{
				if (_compListener.Equals(listener))
				{
					if (_isRunning) Reset();
					
					_compInstanced = false;
				}
			}
		}
	}
	
	private void infCounter(object sender, EventArgs e) { _curCount++; }

    private void limitCounter(object sender, EventArgs e)
	{
		if (_curCount >= _lastCount)
		{
			_core.Tick -= limitCounter;
			_core.Tick += _compEvent;
			_core.Tick += lastFn;
			_compReady = true;
		}
	
		_curCount++;
	}
	
	private void lastFn(object sender, EventArgs e) { Stop(); }
}

} // namespace