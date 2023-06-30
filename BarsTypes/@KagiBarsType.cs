﻿// 
// Copyright (C) 2023, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.ComponentModel;
using NinjaTrader;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
#endregion

namespace NinjaTrader.NinjaScript.BarsTypes
{
	public class KagiBarsType : BarsType
	{
		private enum			Trend					{ Up, Down, Undetermined }
		private double			anchorPrice				= double.MinValue;
		private DateTime		cacheSessionEnd			= Core.Globals.MinDate;
		private bool			endOfBar;
		private DateTime		prevTime				= Core.Globals.MinDate;
		private double			reversalPoint			= double.MinValue;
		private int				tmpCount;
		private int				tmpDayCount;
		private int				tmpTickCount;
		private DateTime		tmpTime					= Core.Globals.MinDate;
		private long			tmpVolume;
		private Trend			trend					= Trend.Undetermined;
		private long			volumeCount;


		public override void ApplyDefaultBasePeriodValue(BarsPeriod period)
		{
			switch (period.BaseBarsPeriodType)
			{
				case BarsPeriodType.Day		: period.BaseBarsPeriodValue = 1;		DaysToLoad = 365;	break;
				case BarsPeriodType.Minute	: period.BaseBarsPeriodValue = 1;		DaysToLoad = 5;		break;
				case BarsPeriodType.Month	: period.BaseBarsPeriodValue = 1;		DaysToLoad = 5475;	break;
				case BarsPeriodType.Second	: period.BaseBarsPeriodValue = 30;		DaysToLoad = 3;		break;
				case BarsPeriodType.Tick	: period.BaseBarsPeriodValue = 150;		DaysToLoad = 3;		break;
				case BarsPeriodType.Volume	: period.BaseBarsPeriodValue = 1000;	DaysToLoad = 3;		break;
				case BarsPeriodType.Week	: period.BaseBarsPeriodValue = 1;		DaysToLoad = 1825;	break;
				case BarsPeriodType.Year	: period.BaseBarsPeriodValue = 1;		DaysToLoad = 15000;	break;
			}
		}

		public override void ApplyDefaultValue(BarsPeriod period)
		{
			period.Value = 2;
		}

		private void CalculateKagiBar(Bars bars, double o, double h, double l, double c, DateTime barTime, long volume)
		{
			switch (trend)
			{
				case Trend.Up:
					if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice - reversalPoint) <= 0)
					{
						AddBar(bars, anchorPrice, anchorPrice, bars.LastPrice, bars.LastPrice, barTime, volumeCount);
						anchorPrice		= bars.LastPrice;
						trend			= Trend.Down;
					}
					else if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice) > 0)
					{
						UpdateBar(bars, bars.LastPrice, l, bars.LastPrice, barTime, volumeCount);
						anchorPrice = bars.LastPrice;
					}
					else
						UpdateBar(bars, h, l, c, barTime, volumeCount);
					break;
				case Trend.Down:
					if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice + reversalPoint) >= 0)
					{
						AddBar(bars, anchorPrice, bars.LastPrice, anchorPrice, bars.LastPrice, barTime, volumeCount);
						anchorPrice = bars.LastPrice;
						trend		= Trend.Up;
					}
					else if (bars.Instrument.MasterInstrument.Compare(bars.LastPrice, anchorPrice) < 0)
					{
						UpdateBar(bars, h, bars.LastPrice, bars.LastPrice, barTime, volumeCount);
						anchorPrice = bars.LastPrice;
					}
					else
						UpdateBar(bars, h, l, c, barTime, volumeCount);
					break;
				default:
					UpdateBar(bars, bars.LastPrice, bars.LastPrice, bars.LastPrice, barTime, volumeCount);
					anchorPrice = bars.LastPrice;
					trend		= bars.Instrument.MasterInstrument.Compare(bars.LastPrice, o) < 0 ? Trend.Down
								: bars.Instrument.MasterInstrument.Compare(bars.LastPrice, o) > 0 ? Trend.Up : Trend.Undetermined;
					break;
			}
			volumeCount = volume;
		}

		public override string ChartLabel(DateTime time)
		{
			switch (BarsPeriod.BaseBarsPeriodType)
			{
				case BarsPeriodType.Day		: return BarsTypeDay.ChartLabel(time);
				case BarsPeriodType.Minute	: return BarsTypeMinute.ChartLabel(time);
				case BarsPeriodType.Month	: return BarsTypeMonth.ChartLabel(time);
				case BarsPeriodType.Second	: return BarsTypeSecond.ChartLabel(time);
				case BarsPeriodType.Tick	: return BarsTypeTick.ChartLabel(time);
				case BarsPeriodType.Volume	: return BarsTypeTick.ChartLabel(time);
				case BarsPeriodType.Week	: return BarsTypeDay.ChartLabel(time);
				case BarsPeriodType.Year	: return BarsTypeYear.ChartLabel(time);
				default						: return BarsTypeDay.ChartLabel(time);
			}
		}

		public override object Clone() { return new KagiBarsType(); }

		public override int GetInitialLookBackDays(BarsPeriod barsPeriod, TradingHours tradingHours, int barsBack)
		{ 
			int minutesPerWeek = 0; 
			lock (tradingHours.Sessions)
			{
				foreach (Session session in tradingHours.Sessions)
				{
					int beginDay	= (int) session.BeginDay;
					int endDay		= (int) session.EndDay;
					if (beginDay > endDay)
						endDay += 7;

					minutesPerWeek += (endDay - beginDay) * 1440 + session.EndTime / 100 * 60 + session.EndTime % 100 - (session.BeginTime / 100 * 60 + session.BeginTime % 100);
				}
			}

			return (int) Math.Max(1, Math.Ceiling(barsBack / Math.Max(1, minutesPerWeek / 7.0 / barsPeriod.Value) * 1.05));
		}

		public override double GetPercentComplete(Bars bars, DateTime now) { return 0; }

		protected override void OnDataPoint(Bars bars, double open, double high, double low, double close, DateTime time, long volume, bool isBar, double bid, double ask)
		{
			if (SessionIterator == null)
				SessionIterator = new SessionIterator(bars);

			#region Building Bars from Base Period
			if (bars.Count != tmpCount) // Reset cache when bars are trimmed
				if (bars.Count == 0)
				{
					tmpTime			= Core.Globals.MinDate;
					tmpVolume		= 0;
					tmpDayCount		= 0;
					tmpTickCount	= 0;
				}
				else
				{
					tmpTime			= bars.GetTime(bars.Count - 1);
					tmpVolume		= bars.GetVolume(bars.Count - 1);
					tmpTickCount	= bars.TickCount;
					tmpDayCount		= bars.DayCount;
					bars.LastPrice	= bars.GetClose(bars.Count - 1);
					anchorPrice		= bars.LastPrice;
				}

			bool isNewSession				= SessionIterator.IsNewSession(time, isBar);
			bool isCalculateTradingDayDone	= false;

			switch (bars.BarsPeriod.BaseBarsPeriodType)
			{
				case BarsPeriodType.Day:
					tmpTime = time.Date; // Will be modified for realtime only
					if (!isBar && time >= cacheSessionEnd /* on realtime includesEndTimeStamp is always false */)
					{
						if (isNewSession)
						{
							SessionIterator.GetNextSession(time, isBar);   
							isCalculateTradingDayDone = true;
						}
						cacheSessionEnd = SessionIterator.ActualSessionEnd;
						if (tmpTime < time.Date) tmpTime = time.Date; // Make sure timestamps are ascending
					}

					if (prevTime != tmpTime) tmpDayCount++;

					if (tmpDayCount < bars.BarsPeriod.BaseBarsPeriodValue
						|| isBar && bars.Count > 0 && tmpTime == bars.LastBarTime.Date
						|| !isBar && bars.Count > 0 && tmpTime <= bars.LastBarTime.Date)
						endOfBar = false;
					else
					{
						prevTime = tmpTime;
						endOfBar = true;
					}

					break;

				case BarsPeriodType.Minute:

					if (tmpTime == Core.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeMinute(bars, time, isBar);

					if (isBar && time <= tmpTime || !isBar && time < tmpTime)
						endOfBar = false;
					else
					{
						prevTime	= tmpTime;
						tmpTime		= TimeToBarTimeMinute(bars, time, isBar);
						endOfBar	= true;
					}
					break;

				case BarsPeriodType.Volume:
					if (tmpTime == Core.Globals.MinDate)
					{
						tmpVolume	= volume;
						endOfBar	= tmpVolume >= bars.BarsPeriod.BaseBarsPeriodValue;
						prevTime	= tmpTime = time;
						if (endOfBar) tmpVolume = 0;
						break;
					}

					tmpVolume	+= volume;
					endOfBar	= tmpVolume >= bars.BarsPeriod.BaseBarsPeriodValue;
					if (endOfBar)
					{
						prevTime	= tmpTime;
						tmpVolume	= 0;
					}
					tmpTime = time;
					break;

				case BarsPeriodType.Tick:
					if (tmpTime == Core.Globals.MinDate || bars.BarsPeriod.BaseBarsPeriodValue == 1)
					{
						prevTime		= tmpTime == Core.Globals.MinDate ? time : tmpTime;
						tmpTime			= time;
						tmpTickCount	= bars.BarsPeriod.BaseBarsPeriodValue == 1 ? 0 : 1;
						endOfBar		= bars.BarsPeriod.BaseBarsPeriodValue == 1;
						break;
					}

					if (tmpTickCount < bars.BarsPeriod.BaseBarsPeriodValue)
					{
						tmpTime			= time;
						endOfBar		= false;
						tmpTickCount++;
					}
					else
					{
						prevTime		= tmpTime;
						tmpTime			= time;
						endOfBar		= true;
						tmpTickCount	= 1;
					}
					break;

				case BarsPeriodType.Month:
					if (tmpTime == Core.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeMonth(time, bars.BarsPeriod.BaseBarsPeriodValue);

					if (time.Month <= tmpTime.Month && time.Year == tmpTime.Year || time.Year < tmpTime.Year)
						endOfBar = false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeMonth(time, bars.BarsPeriod.BaseBarsPeriodValue);
					}
					break;

				case BarsPeriodType.Second:
					if (tmpTime == Core.Globals.MinDate)
					{
						prevTime = tmpTime = TimeToBarTimeSecond(bars, time, isBar);
					}
					if (bars.BarsPeriod.BaseBarsPeriodValue > 1 && time < tmpTime || bars.BarsPeriod.BaseBarsPeriodValue == 1 && time <= tmpTime)
						endOfBar	= false;
					else
					{
						prevTime	= tmpTime;
						tmpTime		= TimeToBarTimeSecond(bars, time, isBar);
						endOfBar	= true;
					}
					break;

				case BarsPeriodType.Week:
					if (tmpTime == Core.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeWeek(time.Date, tmpTime.Date, bars.BarsPeriod.BaseBarsPeriodValue);
					if (time.Date <= tmpTime.Date)
						endOfBar = false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeWeek(time.Date, tmpTime.Date, bars.BarsPeriod.BaseBarsPeriodValue);
					}
					break;

				case BarsPeriodType.Year:
					if (tmpTime == Core.Globals.MinDate)
						prevTime = tmpTime = TimeToBarTimeYear(time, bars.BarsPeriod.Value);
					if (time.Year <= tmpTime.Year)
						endOfBar = false;
					else
					{
						prevTime	= tmpTime;
						endOfBar	= true;
						tmpTime		= TimeToBarTimeYear(time, bars.BarsPeriod.Value);
					}
					break;
			}
			#endregion
			#region Kagi Logic

			reversalPoint = bars.BarsPeriod.ReversalType == ReversalType.Tick ? bars.BarsPeriod.Value * bars.Instrument.MasterInstrument.TickSize : bars.BarsPeriod.Value / 100.0 * anchorPrice;

			if (bars.Count == 0 || IsIntraday && (bars.BarsPeriod.BaseBarsPeriodType != BarsPeriodType.Second && bars.IsResetOnNewTradingDay && isNewSession
													|| bars.BarsPeriod.BaseBarsPeriodType == BarsPeriodType.Second && bars.IsResetOnNewTradingDay && isNewSession))
			{
				if (isNewSession && !isCalculateTradingDayDone)
					SessionIterator.GetNextSession(tmpTime, isBar);

				if (bars.Count > 0)
				{
					double		lastOpen		= bars.GetOpen(bars.Count - 1);
					double		lastHigh		= bars.GetHigh(bars.Count - 1);
					double		lastLow			= bars.GetLow(bars.Count - 1);
					double		lastClose		= bars.GetClose(bars.Count - 1);

					if (bars.Count == tmpCount)
						CalculateKagiBar(bars, lastOpen, lastHigh, lastLow, lastClose, prevTime, volume);
				}

				AddBar(bars, close, close, close, close, tmpTime, volume);
				anchorPrice				= close;
				trend					= Trend.Undetermined;
				prevTime				= tmpTime;
				volumeCount				= 0;
				bars.LastPrice			= close;
				tmpCount				= bars.Count;
				return;
			}

			double	c		= bars.GetClose(bars.Count - 1);
			double	o		= bars.GetOpen(bars.Count - 1);
			double	h		= bars.GetHigh(bars.Count - 1);
			double	l		= bars.GetLow(bars.Count - 1);
			
			if (endOfBar)
				CalculateKagiBar(bars, o, h, l, c, prevTime, volume);
			else
				volumeCount += volume;

			bars.LastPrice			= close;
			tmpCount				= bars.Count;
			#endregion
		}

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Name				= Custom.Resource.NinjaScriptBarsTypeKagi;
				BarsPeriod			= new BarsPeriod { BarsPeriodType = BarsPeriodType.Kagi }; 
				DaysToLoad			= 5;
				DefaultChartStyle	= Gui.Chart.ChartStyleType.KagiLine;
			}
			else if (State == State.Configure)
			{
				switch (BarsPeriod.BaseBarsPeriodType)
				{
					case BarsPeriodType.Minute	: BuiltFrom = BarsPeriodType.Minute; IsIntraday = true; IsTimeBased = true; break;
					case BarsPeriodType.Second	: BuiltFrom = BarsPeriodType.Tick;	IsIntraday = true;	IsTimeBased = true; break;
					case BarsPeriodType.Tick	:
					case BarsPeriodType.Volume	: BuiltFrom = BarsPeriodType.Tick;	IsIntraday = true;	IsTimeBased = false; break;
					default						: BuiltFrom = BarsPeriodType.Day;	IsIntraday = false;	IsTimeBased = true; break;
				}

				switch (BarsPeriod.BaseBarsPeriodType)
				{
					case BarsPeriodType.Day		: Name = string.Format("{0} {1} Kagi{2}",		BarsPeriod.BaseBarsPeriodValue, BarsPeriod.BaseBarsPeriodValue == 1 ? Resource.GuiDaily		: Resource.GuiDay, BarsPeriod.MarketDataType != MarketDataType.Last		? string.Format(" - {0}", BarsPeriod.MarketDataType) : string.Empty);	break;
					case BarsPeriodType.Minute	: Name = string.Format("{0} Min Kagi{1}",		BarsPeriod.BaseBarsPeriodValue, BarsPeriod.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", BarsPeriod.MarketDataType) : string.Empty);																						break;
					case BarsPeriodType.Month	: Name = string.Format("{0} {1} Kagi{2}",		BarsPeriod.BaseBarsPeriodValue, BarsPeriod.BaseBarsPeriodValue == 1 ? Resource.GuiMonthly	: Resource.GuiMonth, BarsPeriod.MarketDataType != MarketDataType.Last	? string.Format(" - {0}", BarsPeriod.MarketDataType) : string.Empty);	break;
					case BarsPeriodType.Second	: Name = string.Format("{0} {1} Kagi{2}",		BarsPeriod.BaseBarsPeriodValue, BarsPeriod.BaseBarsPeriodValue == 1 ? Resource.GuiSecond	: Resource.GuiSeconds, BarsPeriod.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", BarsPeriod.MarketDataType) : string.Empty);	break;
					case BarsPeriodType.Tick	: Name = string.Format("{0} Tick Kagi{1}",		BarsPeriod.BaseBarsPeriodValue, BarsPeriod.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", BarsPeriod.MarketDataType) : string.Empty);																						break;
					case BarsPeriodType.Volume	: Name = string.Format("{0} Volume Kagi{1}",	BarsPeriod.BaseBarsPeriodValue, BarsPeriod.MarketDataType != MarketDataType.Last ? string.Format(" - {0}", BarsPeriod.MarketDataType) : string.Empty);																						break;
					case BarsPeriodType.Week	: Name = string.Format("{0} {1} Kagi{2}",		BarsPeriod.BaseBarsPeriodValue, BarsPeriod.BaseBarsPeriodValue == 1 ? Resource.GuiWeekly	: Resource.GuiWeeks, BarsPeriod.MarketDataType != MarketDataType.Last	? string.Format(" - {0}", BarsPeriod.MarketDataType) : string.Empty);	break;
					case BarsPeriodType.Year	: Name = string.Format("{0} {1} Kagi{2}",		BarsPeriod.BaseBarsPeriodValue, BarsPeriod.BaseBarsPeriodValue == 1 ? Resource.GuiYearly	: Resource.GuiYears, BarsPeriod.MarketDataType != MarketDataType.Last	? string.Format(" - {0}", BarsPeriod.MarketDataType) : string.Empty);	break;
				}

				Properties.Remove(Properties.Find("PointAndFigurePriceType", true));
				Properties.Remove(Properties.Find("Value2", true));

				SetPropertyName("Value", Custom.Resource.NinjaScriptBarsTypeKagiReversal);
			}
		}

		private DateTime TimeToBarTimeMinute(Bars bars, DateTime time, bool isBar)
		{
			if (SessionIterator.IsNewSession(time, isBar))
				SessionIterator.GetNextSession(time, isBar);

			if (bars.IsResetOnNewTradingDay || !bars.IsResetOnNewTradingDay && bars.Count == 0)
			{
				DateTime barTimeStamp = isBar
					? SessionIterator.ActualSessionBegin.AddMinutes(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(SessionIterator.ActualSessionBegin).TotalMinutes)) / bars.BarsPeriod.BaseBarsPeriodValue) * bars.BarsPeriod.BaseBarsPeriodValue)
					: SessionIterator.ActualSessionBegin.AddMinutes(bars.BarsPeriod.BaseBarsPeriodValue + Math.Floor(Math.Floor(Math.Max(0, time.Subtract(SessionIterator.ActualSessionBegin).TotalMinutes)) / bars.BarsPeriod.BaseBarsPeriodValue) * bars.BarsPeriod.BaseBarsPeriodValue);
				if (bars.TradingHours.Sessions.Count > 0 && barTimeStamp > SessionIterator.ActualSessionEnd) // Cut last bar in session down to session end on odd session end time
					barTimeStamp = SessionIterator.ActualSessionEnd;
				return barTimeStamp;
			}
			else
			{
				DateTime lastBarTime	= bars.GetTime(bars.Count - 1);
				DateTime barTimeStamp	= isBar
					? lastBarTime.AddMinutes(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(lastBarTime).TotalMinutes)) / bars.BarsPeriod.BaseBarsPeriodValue) * bars.BarsPeriod.BaseBarsPeriodValue)
					: lastBarTime.AddMinutes(bars.BarsPeriod.BaseBarsPeriodValue + Math.Floor(Math.Floor(Math.Max(0, time.Subtract(lastBarTime).TotalMinutes)) / bars.BarsPeriod.BaseBarsPeriodValue) * bars.BarsPeriod.BaseBarsPeriodValue);
				if (bars.TradingHours.Sessions.Count > 0 && barTimeStamp > SessionIterator.ActualSessionEnd)
				{
					DateTime saveActualSessionEnd = SessionIterator.ActualSessionEnd;
					SessionIterator.GetNextSession(SessionIterator.ActualSessionEnd.AddSeconds(1), isBar);
					barTimeStamp = SessionIterator.ActualSessionBegin.AddMinutes((int) barTimeStamp.Subtract(saveActualSessionEnd).TotalMinutes);
				}
				return barTimeStamp;
			}
		}

		private static DateTime TimeToBarTimeMonth(DateTime time, int periodValue)
		{
			DateTime result = new DateTime(time.Year, time.Month, 1);
			for (int i = 0; i < periodValue; i++)
				result = result.AddMonths(1);

			return result.AddDays(-1);
		}

		private DateTime TimeToBarTimeSecond(Bars bars, DateTime time, bool isBar)
		{
			if (SessionIterator.IsNewSession(time, isBar))
				SessionIterator.GetNextSession(time, isBar);

			if (bars.IsResetOnNewTradingDay || !bars.IsResetOnNewTradingDay && bars.Count == 0)
			{
				DateTime barTimeStamp = isBar
					? SessionIterator.ActualSessionBegin.AddSeconds(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(SessionIterator.ActualSessionBegin).TotalSeconds)) / bars.BarsPeriod.BaseBarsPeriodValue) * bars.BarsPeriod.BaseBarsPeriodValue)
					: SessionIterator.ActualSessionBegin.AddSeconds(bars.BarsPeriod.BaseBarsPeriodValue + Math.Floor(Math.Floor(Math.Max(0, time.Subtract(SessionIterator.ActualSessionBegin).TotalSeconds)) / bars.BarsPeriod.BaseBarsPeriodValue) * bars.BarsPeriod.BaseBarsPeriodValue);
				if (bars.TradingHours.Sessions.Count > 0 && barTimeStamp > SessionIterator.ActualSessionEnd) // Cut last bar in session down to session end on odd session end time
					barTimeStamp = SessionIterator.ActualSessionEnd;
				return barTimeStamp;
			}
			else
			{
				DateTime lastBarTime	= bars.GetTime(bars.Count - 1);
				DateTime barTimeStamp	= isBar
					? lastBarTime.AddSeconds(Math.Ceiling(Math.Ceiling(Math.Max(0, time.Subtract(lastBarTime).TotalSeconds)) / bars.BarsPeriod.BaseBarsPeriodValue) * bars.BarsPeriod.BaseBarsPeriodValue)
					: lastBarTime.AddSeconds(bars.BarsPeriod.BaseBarsPeriodValue + Math.Floor(Math.Floor(Math.Max(0, time.Subtract(lastBarTime).TotalSeconds)) / bars.BarsPeriod.BaseBarsPeriodValue) * bars.BarsPeriod.BaseBarsPeriodValue);
				if (bars.TradingHours.Sessions.Count > 0 && barTimeStamp > SessionIterator.ActualSessionEnd)
				{
					DateTime saveActualSessionEnd = SessionIterator.ActualSessionEnd;
					SessionIterator.GetNextSession(SessionIterator.ActualSessionEnd.AddSeconds(1), isBar);
					barTimeStamp = SessionIterator.ActualSessionBegin.AddSeconds((int) barTimeStamp.Subtract(saveActualSessionEnd).TotalSeconds);
				}
				return barTimeStamp;
			}
		}

		private static DateTime TimeToBarTimeWeek(DateTime time, DateTime periodStart, int periodValue)
		{
			return periodStart.Date.AddDays(Math.Ceiling(Math.Ceiling(time.Date.Subtract(periodStart.Date).TotalDays) / (periodValue * 7)) * (periodValue * 7)).Date;
		}

		private static DateTime TimeToBarTimeYear(DateTime time, double periodValue)
		{
			DateTime result = new DateTime(time.Year, 1, 1);
			for (int i = 0; i < periodValue; i++)
				result = result.AddYears(1);

			return result.AddDays(-1);
		}
	}
}
