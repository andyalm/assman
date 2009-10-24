/*
ImagePreviewBubbleState - An enumeration of the states that a ImagePreviewBubble can be in.
*/
function ImagePreviewBubbleState() {}
ImagePreviewBubbleState.Stopped = 0;
ImagePreviewBubbleState.BubbleUp = 1;
ImagePreviewBubbleState.BubbleDown = 2;


/*
ImagePreviewBubbleManager - Manages the ImagePreviewBubble's in the current page.
*/
function ImagePreviewBubbleManager() {}

ImagePreviewBubbleManager.bubbles = new Array();
ImagePreviewBubbleManager.imageIndex = new Object();
ImagePreviewBubbleManager.bubbleIndex = 0;
ImagePreviewBubbleManager.bubbleDelay = 250;

///<summary>
///Registers the bubble with the given id on the page.
///</summary>
ImagePreviewBubbleManager.registerBubble = function(bubbleId, imageId)
{
	var bubble = new ImagePreviewBubble(bubbleId, imageId);
	ImagePreviewBubbleManager.bubbles.push(bubble);
	$BubbleTrace.writeLine("Bubble '" + bubbleId + "' registered.");
};

///<summary>
///Causes the ImagePreviewBubble to appear for the given image with the given options
///</summary>
ImagePreviewBubbleManager.bubbleImage = function(imageId, options)
{
	ImagePreviewBubbleManager.bubbleDownAll();
	var bubble = ImagePreviewBubbleManager._getBubble();
	if(!bubble)
		throw "A ImagePreviewBubble could not be found on the page.";
	ImagePreviewBubbleManager.imageIndex[imageId] = bubble;
	bubble.bubbleUp(imageId, options, ImagePreviewBubbleManager.bubbleDelay);
};

///<summary>
///Cancels the image preview bubble from appearing for the given image.  This method call
///will only work if the delay has not yet elapsed to have the bubble appear.
///</summary>
ImagePreviewBubbleManager.cancelBubble = function(imageId)
{
	var bubble = ImagePreviewBubbleManager._getBubbleByImageId(imageId);
	if(bubble)
		bubble.cancelBubbleTimer();
};

///<summary>
///Causes all of the ImagePreviewBubble's on the page to bubble down.
///</summary>
ImagePreviewBubbleManager.bubbleDownAll = function()
{
	ImagePreviewBubbleManager.bubbles.each(function(bubble)
	{
		bubble.bubbleDown();
	});
};

ImagePreviewBubbleManager._getBubble = function()
{
	if(ImagePreviewBubbleManager.bubbleIndex >= ImagePreviewBubbleManager.bubbles.length)
		ImagePreviewBubbleManager.bubbleIndex = 0;
	return ImagePreviewBubbleManager.bubbles[ImagePreviewBubbleManager.bubbleIndex++];
};

ImagePreviewBubbleManager._getBubbleByImageId = function(imageId)
{
	return ImagePreviewBubbleManager.imageIndex[imageId];
};

///<summary>
///Creates a new instance of an ImagePreviewBubble.
///</summary>
function ImagePreviewBubble(bubbleId, imageId)
{
	this.bubbleId = bubbleId;
	this.container = document.getElementById(bubbleId);
	this.image = document.getElementById(imageId);
	this.link = this.image.parentNode;
	this.state = ImagePreviewBubbleState.Stopped;
	this.deferredBubble = new AjaxExtender.DeferredOperation(0, null, null); //this instance isn't used, just here to avoid null reference
	this.topPadding = 15;
	this.rightPadding = 17;
	this.bottomPadding = 23;
	this.leftPadding = 17;
	this.animationTime = 0.5;
	this.animationFps = 25;
		
	this.animator = new BubbleAnimation(this, this.animationTime, this.animationFps);
	this.animator.add_ended(this._onStopped.bind(this));
	_eventManager.addDOMListener("mouseleave", this.bubbleDown.bind(this), this.container);
}

ImagePreviewBubble.prototype.bubbleUp = function(imgId, options, delay)
{
	this.stop();
	this.state = ImagePreviewBubbleState.BubbleUp;
	var delegate = this._doBubbleUp.bind(this, imgId, options);
	if(delay > 0.0)
	{
		this.deferredBubble = new AjaxExtender.DeferredOperation(delay, null, delegate);
		this.deferredBubble.post();
	}
	else
	{
		delegate();
	}
};

ImagePreviewBubble.prototype._doBubbleUp = function(imageId, options)
{
	var image = document.getElementById(imageId);
	if(!image)
		return;	
		
	//ensure we have a non-null options
	options = options || {};
	
	this._fillEmptyOptions(options, image.src);
	this._setBeginDimensions(image, options);
	this._setFullDimensions(options);
	
	//init animator
	this.animator.clearEffects();
	this.animator.addEffect(new BubbleFadeEffect(this), options.fade);
	this.animator.addEffect(new BubbleZoomEffect(this), options.zoom);
	
	//init styles
	if(options.navigateUrl)
		this.link.setAttribute("href", options.navigateUrl);
	else
		this.link.removeAttribute("href");
	this.image.src = options.imageUrl;
	this.container.style.display = "block";
	this.animator.playUp();
};

ImagePreviewBubble.prototype.cancelBubbleTimer = function()
{
	if(this.deferredBubble.get_isPending())
	{
		this.state = ImagePreviewBubbleState.Stopped;
		this._trace("state is stopped because of a cancelled timer");
		this.deferredBubble.cancel();
	}
};

ImagePreviewBubble.prototype.bubbleDown = function()
{
	if(this.state != ImagePreviewBubbleState.BubbleUp)
		return;
	this.animator.pause();
	
	this.state = ImagePreviewBubbleState.BubbleDown;
	this.animator.playDown();
};

ImagePreviewBubble.prototype.stop = function()
{
	if(this.state == ImagePreviewBubbleState.Stopped)
		return;
	this.cancelBubbleTimer();
	this.animator.stop();
};

ImagePreviewBubble.prototype._fillEmptyOptions = function(options, defaultImageUrl)
{
	if(!options.imageUrl)
		options.imageUrl = defaultImageUrl;
	if(options.fade != false)
		options.fade = true;
	if(options.zoom != false)
		options.zoom = true;
	if(!options.offsetTop)
		options.offsetTop = 0;
	if(!options.offsetLeft)
		options.offsetLeft = 0;
	if(!options.offsetViewPortWidth)
		options.offsetViewPortWidth = 0;
	if(options.adjustForScrollPosition != false)
		options.adjustForScrollPosition = true;
	if(options.posContainerId)
	{
	    options.posContainer = document.getElementById(options.posContainerId);
	}
	else
	{
		if(document.body.scrollLeft)
			options.posContainer = document.body;
		else
			options.posContainer = document.documentElement;
	}
		
	return options;
};

ImagePreviewBubble.prototype._setBeginDimensions = function(image, options)
{
	var pos = WebForm_GetElementPosition(image);
	var scrollOffset = { x : 0, y : 0 };
	if(options.adjustForScrollPosition)
		scrollOffset = this._getScrollOffset(image);
	this.beginWidth = pos.width;
	this.beginHeight = pos.height;
	this.beginTop = pos.y - scrollOffset.y - this.topPadding;
	this.beginLeft = pos.x - scrollOffset.x - this.leftPadding;
	
	this._trace("beginTop: " + this.beginTop + " | beginLeft: " + this.beginLeft);
};

ImagePreviewBubble.prototype._setFullDimensions = function(options)
{
	var pageWidth = options.posContainer.offsetWidth + options.posContainer.scrollLeft + options.offsetViewPortWidth;
		
	if(this.beginWidth > this.beginHeight)
	{
		this.fullWidth = options.maxDimension || this.beginWidth * 2;
		this.fullHeight = this.beginHeight * (this.fullWidth / this.beginWidth);
	}
	else
	{
		this.fullHeight = options.maxDimension || this.beginHeight * 2;
		this.fullWidth = this.beginWidth * (this.fullHeight / this.beginHeight);
	}
	
	//position the bubbled image vertically aligned to the bottom of the thumb
	var adjustedTop = this.beginTop + options.offsetTop - (this.fullHeight - this.beginHeight);
	//position the bubbled image horizontally aligned to the center of the thumb
	var adjustedLeft = this.beginLeft + options.offsetLeft - (this.fullWidth - this.beginWidth) * 0.5;

	var minLeft = options.posContainer.scrollLeft;
	var maxLeft = pageWidth - (this.leftPadding + this.fullWidth + this.rightPadding);
	if(maxLeft < minLeft)
		maxLeft = minLeft;
		
	if(adjustedLeft < minLeft)
		adjustedLeft = minLeft;
	else if(adjustedLeft > maxLeft)
		adjustedLeft = maxLeft;
	if(adjustedTop < 0)
		adjustedTop = 0.0;
		
	adjustedTop = parseInt(adjustedTop);
	adjustedLeft = parseInt(adjustedLeft);
		
	this.fullTop = adjustedTop;
	this.fullLeft = adjustedLeft;
	
	this._trace("fullTop: " + this.fullTop + " | fullLeft: " + this.fullLeft);
};

ImagePreviewBubble.prototype._getScrollOffset = function(element)
{
	var scrollOffset = { x : 0, y : 0 };
	var node = element.parentNode;
    while(node && node.tagName != "BODY")
    {
		if(node.scrollLeft && node.scrollLeft > 0)
			scrollOffset.x += node.scrollLeft;
		if(node.scrollTop && node.scrollTop > 0)
			scrollOffset.y += node.scrollTop;
		node = node.parentNode;
    }
    
    return scrollOffset;
};

ImagePreviewBubble.prototype._onStopped = function()
{
	if(this.animator.isDown())
	{
		this.state = ImagePreviewBubbleState.Stopped;
		this._trace("set to stopped because the bubble down ended");
		this.container.style.display = "none";
		this.image.src = "/images/clear.gif";
	}
};

ImagePreviewBubble.prototype._trace = function(message)
{
	$BubbleTrace.writeLine("Bubble '" + this.bubbleId + "': " + message);	
};

/*

BubbleAnimation class.  This class controls the animation of its bubble effects.

*/

function BubbleAnimation(bubble, duration, fps) //inherits from AjaxExtender.Animation.Animation
{
	BubbleAnimation.initializeBase(this, [null, duration, fps]);
	this.bubble = bubble;
	this.animations = [];
	this.actions = [];
	this.direction = BubbleAnimation.Direction.Up;
	this.add_ended(this._onEnded.bind(this));
}

BubbleAnimation.Direction = { Up : 0, Down : 1 }

BubbleAnimation.prototype.addEffect = function(effect, animate)
{
	if(animate)
		this.animations.push(effect);
	else
		this.actions.push(effect);
};

BubbleAnimation.prototype.clearEffects = function()
{
	this.animations = [];
	this.actions = [];
};

BubbleAnimation.prototype.playUp = function()
{
	this.direction = BubbleAnimation.Direction.Up;
	this.actions.each(function(action) { action.render(1.0); });
	this.play();
};

BubbleAnimation.prototype.playDown = function()
{
	this.direction = BubbleAnimation.Direction.Down;
	this.play();
};

BubbleAnimation.prototype.isDown = function()
{
	return this.direction == BubbleAnimation.Direction.Down;
};

BubbleAnimation.prototype.getAnimatedValue = function(percentage)
{
	var dec = percentage * .01;
	if(this.direction == BubbleAnimation.Direction.Up)
		return dec;
	else
		return 1 - dec;
};

BubbleAnimation.prototype.setValue = function(value)
{
	this.animations.each(function(effect) { effect.render(value); });
};

BubbleAnimation.prototype._onEnded = function()
{
	if(this.isDown())
		this.actions.each(function(action) { action.render(0.0); });
};

if(typeof(AjaxExtender) != "undefined" && typeof(AjaxExtender.Animation) != "undefined")
	BubbleAnimation.registerClass('BubbleAnimation', AjaxExtender.Animation.Animation);

/*

Bubble effects

*/

/*
The bubble fade effect makes the bubble fade in and out
*/
function BubbleFadeEffect(bubble)
{
	this.bubble = bubble;
}

BubbleFadeEffect.prototype.render = function(value)
{
	Element_SetOpacity(this.bubble.container, value);
};

/*
The bubble zoom effect makes the bubble grow out of the thumbnail into the the comp size.
*/
function BubbleZoomEffect(bubble)
{
	this.bubble = bubble;
}

BubbleZoomEffect.prototype.render = function(value)
{
	var top = parseInt(this.bubble.beginTop + (this.bubble.fullTop - this.bubble.beginTop) * value);
	var left = parseInt(this.bubble.beginLeft + (this.bubble.fullLeft - this.bubble.beginLeft) * value);
	var width = parseInt(this.bubble.beginWidth + (this.bubble.fullWidth - this.bubble.beginWidth) * value);
	var height = parseInt(this.bubble.beginHeight + (this.bubble.fullHeight - this.bubble.beginHeight) * value);
	
	this.bubble.image.style.width = width + "px";
	this.bubble.image.style.height = height + "px";
	this.bubble.container.style.top = top + "px";
	this.bubble.container.style.left = left + "px";
};

function ImagePreviewBubbleTrace() {}

//alias for tracing to the ImagePreviewBubble event log
var $BubbleTrace = ImagePreviewBubbleTrace;

ImagePreviewBubbleTrace.writeLine = function(message)
{
	Trace.writeLine(message, "ImagePreviewBubble");
};