

function Dependency() { }

Dependency.markerIsPresent = function ()
{
	return $("#script-include-dependencies").length > 0;
};

Dependency.markPassed = function ()
{
	$("#script-include-dependencies").addClass("pass");
};