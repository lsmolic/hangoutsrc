function DetectUnityWebPlayerActiveX
	on error resume next
	dim tControl, res, ua, re, matches, major
	res = 0
	set tControl = CreateObject("UnityWebPlayer.UnityWebPlayer.1")
	if IsObject(tControl) then
		if tControl.GetPluginVersion() = "2.5.0f5" then
			' 2.5.0f5 on Vista and later has an auto-update issue
			' on Internet Explorer. Detect Vista (6.0 or later)
			' and in that case treat it as not installed
			ua = Navigator.UserAgent
			set re = new RegExp
			re.Pattern = "Windows NT (\d+)\."
			set matches = re.Execute(ua)
			if matches.Count = 1 then
				major = CInt(matches(0).SubMatches(0))
				if major < 6 then
					res = 1
				end if
			end if
		else
			res = 1
		end if
	end if
	DetectUnityWebPlayerActiveX = res
end function

