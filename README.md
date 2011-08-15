Assman
=========
Frictionless Web Asset Management for ASP.NET MVC.

Feature Overview
------------------------
* [Manages dependencies between scripts and automatically includes the dependencies when you request a script on the page](http://assman.codeplex.com/wikipage?title=Dependency-Management)
* [Scripts and Styles consolidated by convention](http://assman.codeplex.com/wikipage?title=Compilation) (you define the convention that works for your project), and thus requires minimal configuration/maintenence
* [Minifies your scripts and stylesheets](http://assman.codeplex.com/wikipage?title=Minification)
* [Manages the inclusion/rendering of Scripts and Stylesheets](http://assman.codeplex.com/wikipage?title=Registration) so that they can be rendered in the most optimal section of the page
* [Contains a hook to support including resources from a CDN when your site uses one](http://assman.codeplex.com/wikipage?title=CDN)
* [Extensible plugin architecture](http://assman.codeplex.com/wikipage?title=Plugins) to support extending the library (CoffeeScript and dotLess plugins already exist)
* [Optimizes development experience in Debug mode, optimizes website performance in Release mode]http://assman.codeplex.com/wikipage?title=ResourceMode)
* Does not require any tools to be installed

Getting Started
--------------------

*NOTE: These instructions are for Mvc3.  If you are using Mvc2, please see these [instructions|Mvc2]*
1. Install the Assman.Mvc3 NuGet package:

	Install-Package Assman.Mvc3

2. While Assman ships with a light javascript minifier (JSMin), it does not ship with a css minifier.  If you want to enable css minification as well as better js minification, then install the Assman.YuiCompressor NuGet package

	Install-Package Assman.YuiCompressor

3. Open your site's layout/master page, and add the following inside the <head> element of your page:

	@Html.RenderStylesheets()
	@Html.RenderScripts("head")

4. Also add the following line right before the closing body tag

	@Html.RenderScripts()

5. To include a stylesheet on your page, you can place this anywhere in a view or partial view:

	@Html.RequireStylesheet("~/path/to/my/stylesheet.css")

6. To include a script at the bottom of your page (preferred):

	@Html.RequireScript("~/path/to/my/script.js")

7. If you would like an individual script to be included in the head of your page, you can call RequireScript and specify "head" like this:

	@Html.RequireScript("~/path/to/my/script.js", "head")

For more info/recomendations on best practices/locations for including scripts, [click here](http://assman.codeplex.com/wikipage?title=ScriptLocation)

How It Works
------------------

### Consolidation/Mashing by convention

This library provides a configuration section where you can create groups in which your .js and .css files will be included on your page.  It supports defining your own conventions so that you hopefully don't have to maintain this file very much.  Here is an example of a config file where conventions are defined for an MVC project to consolidate the scripts and stylesheets into one file per controller:

	<assman>
		<scripts>
        	<groups>
				<group consolidatedUrl="~/Scripts/Consolidated/Mvc/{controller}.js">
      				<include>
        				<add regex="~/Views/(?'controller'\w+)/.+" />
      				</include>
    			</group>
	    	</groups>
    	</scripts>
		<stylesheets>
        	<groups>	
        		<group consolidatedUrl="~/Content/Consolidated/Mvc/{controller}.css">
					<include>
						<add regex="~/Views/(?'controller'\w+)/.+" />
					</include>
				</group>
        	</groups>
        </stylesheets>
	</assman>

### Manages and respects dependencies

If you are including a script on your page called ~/Views/MyViewBootstrapper.js, and it depends on ~/scripts/mywidget.js, Assman will automatically ensure that mywidget.js gets included on the page ahead of MyViewBootstrapper.js.  The way you indicate dependencies for scripts is by including this at the top of your .js file:

	///<reference path="~/scripts/mywidget.js" />

If this looks familiar, that is because this is the same syntax Visual Studio uses to support its javascript Intellisense.  Note that if MyWidget.js and MyViewBootstrapper.js get consolidated into the same .js file, then it will ensure that the content from MyWidget.js appears above the content for MyViewBootstrapper.js in the consolidated file.  If you have a css stylesheet that depends on another stylesheet already being present, you can declare it at the top of your .css file in a comment block like this:

	/*
     	dependency:url(/Content/SharedStyles.css);
	*/

Why this library exists
-----------------------------

As the web progresses, a significant percentage of the code for a site is being written in javascript.  As a result, being able to manage your scripts (and stylesheets) is becoming critical.  From the developer's point of view, you want to be able to organize your code into lots of files and freely include comments in your scripts where necessary.  However, having lots of script includes and comments in the scripts has a significant effect on performance for the end user. (more round trips for the browser and larger files to download).  What is ideal then, is to be able to break out your scripts into as many files as you want, adding as many comments as you would like, and then put your scripts through a packaging process so that when the website runs, it includes all of your scripts in a few consolidated files with all of the comments and insignificant whitespace taken out).  Out of the box, ASP.NET does not give you much help in this area, and while there are plenty of libraries out there that can compress and consolidate scripts, automating the process in a way that does not introduce friction to the developer is not always easy.  In addition, managing the dependencies between your scripts manually is tedious and error prone. This library exists to make the management of your web assets seemless so that the developer can focus on writing the code and organizing it how he/she wants.

