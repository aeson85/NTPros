(function(window){
	var Module = function(){
		var self = this;
		self.modMap = {};	
		self.modArr = [
			"header-nav"
		];

		this.init = function(callback){
			this.initModule(callback);
		}	

		this.getModule = function(mod,retrieval,callback){
			var url = "../module/" + mod + ".htm";
			$.get(url,null,function(msg){
				var module = {};
				module.html = msg;
				module.getRet = function(){
					var dom = $(module.html)[0];
					var children = {};
					for(var i=0;i<retrieval.length;i++){
						$(dom).find(retrieval[i]).each(function(){
							var key = this.dataset.key;
							children[key] = this;
						})
					}
					return children;
				}
				callback(module);
			});
		}

		this.initModule = function(callback){
			var count = 0;
			var getMod = function(mod){
				if(mod){
					self.modMap[self.modArr[count++]] = mod;
					$('#ntLoad span').html(Math.floor(count/self.modArr.length)*100 + "%");
				}
				if(count == self.modArr.length)	{
					callback();
					return;	
				}
				self.getModule(self.modArr[count],["div","span","label","li","audio","video","input","img","a","img"],getMod);
			}
			getMod();

		}
	}

	window.mod = Module;	
 })(window);

(function(){
	nt.mod = new mod();
	nt.mod.init(function(){
		var container = $("[data-module='header-nav']")[0];
		container.innerHTML = nt.mod.modMap["header-nav"].html;
		$("[data-url-switch='true']").each(function(){
			this.dataset.target = navMap[this.dataset.url];
			if(this.dataset.url == nt.page){
				$(this).addClass("nav-on");
			}
		});
		
		if(nt.page == 0){return;}
		$(".nav-ico").on("tap",function(){
			window.location.href = this.dataset.target;
		});
		
		window.setTimeout(function(){
			var load = $(".nt-loading")[0];
			load.style['opacity'] = 0;
			load.addEventListener('webkitTransitionEnd',function(){
				$(this).hide();
			},false);
		},300);
	});
})();

/*swipe control*/
(function(){
	//var overscroll = function(el) {
		//el.addEventListener('touchstart', function() {
			//var top = el.scrollTop
			//, totalScroll = el.scrollHeight
			//, currentScroll = top + el.offsetHeight;
		//if(top === 0) {
			//el.scrollTop = 1;
		//} else if(currentScroll === totalScroll) {
			//el.scrollTop = top - 1;
		//}
		//});
		//el.addEventListener('touchmove', function(evt) {
			//if(el.offsetHeight < el.scrollHeight){
				//evt._isScroller = true;
			//}
		//});
	//}

	//document.body.addEventListener('touchmove', function(evt) {
		//if(!evt._isScroller) {
			//evt.preventDefault();
		//}
	//});
	
	//overscroll($('.page')[0]);

	//if(nt.page == 0){
		//overscroll($(".act-panel")[0]);
	//}
})();


var swipeOn = false;
(function(window){
	$("#userIco,#swipeIco").on("tap",function(){
		var rem = swipeOn?0:parseInt(this.dataset.position,10);
		$(".page").css("-webkit-transform","translate3d("+rem+"rem,0,0)");
		if(nt.page == 0){
			$(".swipe-right-card-box").css("right",(swipeOn?-6:0)+"rem");
		}
		swipeOn = !swipeOn;
	});

	$( document ).on( "swipeleft swiperight", ".page", function(e) {
		e.preventDefault();
		e.stopPropagation();
		var rem = swipeOn?0:((e.type=="swiperight")?6:-8);
		$(".page").css("-webkit-transform","translate3d("+rem+"rem,0,0)");
		swipeOn = !swipeOn;
	});


	$(".panel-close").on("tap",function(){
		$('.panel-bg').hide();
		$('.'+this.dataset.panel).hide();
	});

	if(nt.page != 0){
		$("#userIco,#swipeIco").css("color","#4e4e4e");
	}
})(window);


/*nav control*/
(function(){
})();

(function(){
	if(nt.page != 0){return;}

	$("#actCalendar").on("tap",function(){
		$("#calendarBG").show();
		$("#calendar").show();
		$("#calendar")[0].scrollTop = 0;
	});
	
	$("#swipeIco").on("tap",function(){
		$(".swipe-right-card-box").css("right",(swipeOn?0:-3)+"rem");
		$('.act-card-info-panel')[(swipeOn)?"show":"hide"]();
	});

	$("#userIco").on("tap",function(){
		var rem = swipeOn?-8:-3;
		$(".swipe-right-card-box").css("right",rem+"rem");
	});

	$( document ).on( "swipeleft swiperight", ".page", function(e) {
		var rem = (e.type == "swiperight")?(swipeOn?-8:-3):(swipeOn?0:-3);
		$(".swipe-right-card-box").css("right",rem + "rem");
		$('.act-card-info-panel')[(swipeOn && e.type != "swiperight")?"show":"hide"]();
	});

	$(".active-card").on("tap",function(){
		$(".page").css("-webkit-transform","translate3d(-8rem,0,0)");
		$(".swipe-right-card-box").css("right","0rem");
		swipeOn = true;
		$('.act-card-info-panel')[(swipeOn)?"show":"hide"]();
	});

	$(".m-nav").on("tap",function(){
		console.log(this.dataset.target);
		window.location.href = this.dataset.target;
	});

	$(document.body).infinite();
})();

(function(){
	if(nt.page != 1){return;}

})();



(function($){
	var getWClientHeight = function(){
		if(document.compatMode == 'BackCompat'){
			return document.body.clientHeight;
		}
		if(document.compatMode == 'CSS1Compat'){
			return document.documentElement.clientHeight;
		}
	}




	if(nt.page == 2){

		var data = [
		{url:"/img/rec01.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"},
		{url:"/img/rec08.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"},
		{url:"/img/rec02.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"},
		{url:"/img/rec03.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"},
		{url:"/img/rec04.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"},
		{url:"/img/rec05.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"},
		{url:"/img/rec06.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"},
		{url:"/img/rec07.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"},
		{url:"/img/rec09.jpg",op:"￥32",mp:"￥42",title:"NT Brunch"}
		];

		var createItem = function(item,data,next){
			var eleBG = nt.widget.createDom("div:ele-bg",null,null,item);
			eleBG.style['background-image'] = "url("+data.url+")";
			var usrBar = nt.widget.createDom("div:usr-bar",null,null,item); 
			var usrBarBG = nt.widget.createDom("div:usr-bar-bg",null,null,usrBar); 
			var op= nt.widget.createDom("h2",null,{html:data.op},usrBar); 
			var mp = nt.widget.createDom("span",null,{html:data.mp},op); 
			var title = nt.widget.createDom("p",null,{html:data.title},usrBar); 
			var ico = nt.widget.createDom("a:fa fa-bars usr-mark-ico-right",null,null,usrBar); 
			$(ico).attr("aria-hidden","true");

			var img = new Image();
			img.src = data.url;
			var sw,sh;
			img.onload = function(){
				sw = this.width;	
				sh = this.height;
				var w = nt.widget.getRect(item).width; 
				var h =  w/sw*sh;
				h = h<w?w:h;
				eleBG.style["width"] = "100%";
				eleBG.style["height"] = h + "px";

				next(h);	
			}

		}

		var container = $('#waterfall')[0];
		var configure = {
			offset:0,
			columnWidth:$(window).width()/2,
			gutter:0,
			loadCount:self.PageSize,
			itemSelector:'ele-item'
		};

		var waterfall = new nt.widget.waterfall(data,configure,container,function(entity,item,i,next){
			createItem(item,entity.data[i],function(h){
				next(h);
			});
		},null,null);

	}
})($);


$(document).ready(function(){
	$(document).on ( "vmousemove",function(event) {
		//var scrollTop = $('.page-current').scrollTop();
		//if(scrollTop > 0){
		//$("#barNavBG")[0].className = "nav-theme-dark";
		//}else{
		//$("#barNavBG")[0].className = "nav-theme-light";
		//}

		//event.preventDefault();
		//event.stopPropagation();
	});
	//var scene = document.getElementById('scene');
	//var parallaxInstance = new Parallax(scene);
	//parallaxInstance.scalar(10,5);
});
