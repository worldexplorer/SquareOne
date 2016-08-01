using System;

using Newtonsoft.Json;

namespace Sq1.Widgets.Exceptions {
	public class ExceptionsControlDataSnapshot {
		[JsonProperty]	public int		SplitDistanceVertical				= 0;
		[JsonProperty]	public int		SplitDistanceHorizontal				= 0;
		[JsonProperty]	public bool		RecentAlwaysSelected				= true;
		[JsonProperty]	public bool		ShowTimestamps						= false;
		[JsonProperty]	public bool		ShowHeaders							= false;
		[JsonProperty]	public bool		PopupOnIncomingException			= false;
		[JsonProperty]	public bool		ShowTimesOccured					= false;

		[JsonProperty]	public int		FlushToGuiDelayMsec					= 500;
		[JsonProperty]	public float	LogRotateSizeLimit_Mb				= 10.0f;

		[JsonProperty]	public bool		ShowSearchbar						= false;
		[JsonProperty]	public string	ShowSearchbar_ExcludeKeywordsCsv	= "";
		[JsonProperty]	public bool		ShowSearchbar_SearchKeywordApplied	= false;
		[JsonProperty]	public string	ShowSearchbar_SearchKeywordsCsv		= "";
		[JsonProperty]	public bool		ShowSearchbar_ExcludeKeywordApplied	= false;
	}
}
