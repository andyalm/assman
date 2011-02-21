///<reference path="~/scripts/Dependency.js" />

$(document).ready(function ()
{
	if (Dependency.markerIsPresent())
		Dependency.markPassed();
});
