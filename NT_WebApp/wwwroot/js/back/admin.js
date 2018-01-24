(function(window){
	var admin = {};
	admin.curPage = null;
	window.admin = admin;
})(window);

(function(){
	var widget = function(){
		this.getWClientHeight = function(){
			if(document.compatMode == 'BackCompat'){
				return document.body.clientHeight;
			}
			if(document.compatMode == 'CSS1Compat'){
				return document.documentElement.clientHeight;
			}
		}

		this.getWClientWidth = function(){
			if(document.compatMode == 'BackCompat'){
				return document.body.clientWidth;
			}
			if(document.compatMode == 'CSS1Compat'){
				return document.documentElement.clientWidth;
			}
		}

		this.createDom = function(tag,id,pnode){
			var ts = tag.split(':');
			if(ts[0] == 'img'){
				var dom = new Image();	
			}else{
				var dom = document.createElement(ts[0]);	
			}
			dom.className = ts[1];
			if(id){dom.id = id;}
			if(pnode){
				pnode.appendChild(dom);	
			}
			return dom;
		}

		this.subString = function(str,len){
			var s = str;	
			if(s.replace(/[^\x00-\xff]/g, "**").length>len){
				while(s.replace(/[^\x00-\xff]/g, "**").length>len){
					var s = s.substring(0,s.length-1);
				}
				return s+'...';
			}else{
				return s;
			}
		}

		this.execCommand = function(btn,val){
			$(btn).click(function(){
				var txt= admin.widget.createDom("textarea",null,document.body);
				txt.innerHTML = val;
				txt.select();
				document.execCommand("Copy"); 
				$(txt).remove();
			});
		}

		this.registEnterKeyEvent = function(dom,callback){
			if(!dom){return;}
			dom.onkeydown = function(e){
				if(e.keyCode == '13'){
					e.preventDefault();
					e.stopPropagation();
				}
			}
			dom.onkeyup = function(e){
				if(e.keyCode == '13'){
					e.preventDefault();
					e.stopPropagation();
					dom.blur();
					if(callback){
						callback();	
					}
				}
			}
		}



	}
	admin.widget = new widget();	
})();


(function(){
	var IMGEditor = function(){
		var self = this;
		var menuMap = {
			"AllIMG":"所有图片",
			"OrderIMG":"点餐区",
			"Activity":"分享会",
			"Breakfast":"早餐",
			"Package":"套餐活动",
			"Salad":"沙拉，健身餐",
			"ShelfGoods":"货架商品",
			"Coffee":"咖啡",
			"Drinks":"其他饮品"
		}
		this.init = function(){
			this.initMenuTree();	
		}

		this.initMenuTree = function(){
			//$.get("http://47.104.84.30/api/ftp/",null,function(data){
			$.get("/api/ftp/",null,function(data){
				var container = $("#menuTree")[0];
				var tree = [];
				for(var i in menuMap){
					$(data).each(function(){
						if(this.name == i){
							tree.push(this);
						}
					});
				}

				$(tree).each(function(){
					var li = admin.widget.createDom("li:tree-off",null,container);
					li.dataset.name = this.name;
					var ico = admin.widget.createDom("i:fa fa-folder-o",null,li);
					$(ico).attr("aria-hidden","true");
					var txt = admin.widget.createDom("label",null,li);
					txt.innerHTML = menuMap[this.name];
					$(li).click(self.getImgFolder);
				});
				$("#menuTree li:eq(0)").click();
			});
		}


		this.getImgFolder = function(){
			$(".menu-tree li").each(function(){
				this.className = "tree-off";
			});
			this.className = "tree-on";

			var container = $("#imgsPage")[0];
			container.innerHTML = '';
			//$.get("http://47.104.84.30/api/ftp/"+this.dataset.name,null,function(data){
			$.get("/api/ftp/"+this.dataset.name,null,function(data){
				$(data).each(function(i){
					var viewBox = admin.widget.createDom("div:img-view-box",null,container);
					var view = admin.widget.createDom("div:img-view",null,viewBox);
					var name = admin.widget.createDom("div:img-view-name",null,viewBox);
					name.innerHTML = this.name;

					var imgUrl = admin.widget.createDom("div:img-view-url",null,viewBox);
					var url = this.url;
					//imgUrl.innerHTML = admin.widget.subString(url,18); 
					//var url = "http://47.104.84.30"+this.url;
					imgUrl.innerHTML = admin.widget.subString(url,18); 

					var ico = admin.widget.createDom("i:fa fa-clone",null,imgUrl);
					$(ico).attr("aria-hidden","true");
					admin.widget.execCommand(ico,url);

					var img = new Image();
					img.src = url;
					img.onload = function(){
						var w = this.width;	
						var h = this.height;
						if(w>h){
							view.style["background-size"] = "100% auto";
						}else{
							view.style["background-size"] = "auto 100%";
						}
						view.style["background-image"] = "url("+url+")";
					}


				})
			})
		}


	}	
	admin.IMGEditor = new IMGEditor();
})();


(function(){
	var NTGoodsEditor = function(){
		//this.TmpData = {
			//"Name": "Name",
			//"Images": [{
				//"Url": "Url1",
				//"Width": "i00",
				//"Height": "200",
				//"Type": 1
			//},{
				//"Url": "Url2",
				//"Width": "100",
				//"Height": "200",
				//"Type": 2
			//}],
			//"Title": "Title",
			//"Introduction": "Introduction",
			//"Details": "Details",
			//"Prices": {
			//"Original": 1.0,
			//"Present" : 2.0,
			//"Membership": 3.0
			//},
			//"Type": 1,
			//"Group": 2,
			//"PubType": 3,
			//"CanCollection": true,
			//"ResDateStart": "2018-1-17",
			//"ResDateEnd": "2018-2-17",
			//"Widget": "Widget"
		//};

		
		var self =this;
		this.init = function(){
			this.initInfoData();
			this.initInfoEditor();
		}	

		this.initInfoData = function(){
			this.TmpData = {};	
			this.TmpData["Name"] = "";
			this.TmpData["Images"] = [
			{"Url":"","Width":null,"Height":null,"Type":0},
			{"Url":"","Width":null,"Height":null,"Type":1},
			{"Url":"","Width":null,"Height":null,"Type":1}
			];

			this.TmpData["Title"] = "";
			this.TmpData["Introduction"] = "";
			this.TmpData["Details"] = "";
			this.TmpData["Prices"] = {};
			this.TmpData["Prices"]["Original"] = 0;
			this.TmpData["Prices"]["Present"] = 0;
			this.TmpData["Prices"]["Membership"] = 0;
			this.TmpData["Type"] = 0;
			this.TmpData["Group"] = 0;
			this.TmpData["PubType"] = 0;
			this.TmpData["CanCollection"] = false;
			this.TmpData["ResDateStart"] = "1990-1-1";
			this.TmpData["ResDateEnd"] = "1990-1-1";
			this.TmpData["Widget"] = "";
		}



		this.initInfoEditor = function(){
			var _table = $("#gInfoForm")[0];
			var ctl = {
				init:function(){
					this.registEvent();	
					this.registTextEvent();
					this.registRadioEvent();
					this.registTextareaEvent();
				},
				registTextEvent:function(){
					$(_table).find("input[type='text']").each(function(){
						$(this).keyup(function(){
							self.TmpData[this.dataset.flag] = this.value;
						});
					});
				},
				clearText:function(){
					$(_table).find("input[type='text']").each(function(){
						this.value = "";	
						self.TmpData[this.dataset.flag]
					});
				},
				registRadioEvent:function(){
					$(_table).find("input[type='radio']").click(function(){
						if(this.name == "CanCollection"){
							self.TmpData[this.name] = (this.value=="0")?false:true;
						}else{
							self.TmpData[this.name] = this.value;
						}
					});
				},
				clearRadio:function(){
					$(_table).find("input[type='radio']").each(function(){
						if(this.name == "CanCollection"){
							if(this.value == "0"){
								this.checked = true;
								self.TmpData[this.name] = false;
							}
						}else{
							this.checked = false;
							self.TmpData[this.name] = 0;
						}
					});
				},
				registTextareaEvent:function(){
					$(_table).find("div[contenteditable='true']").each(function(){
						$(this).keyup(function(){
							self.TmpData[this.dataset.flag] = escape(this.innerHTML);
							console.log(self.TmpData)
						});
					});
				},
				clearTextarea:function(){
					$(_table).find("div[contenteditable='true']").each(function(){
						this.innerHTML = "";	
						self.TmpData[this.dataset.flag]
					});
				},
				registEvent:function(){
					$("#clearGEdit").click(this.clear);
					$("#submitGEdit").click(this.submit);
					$(".g-view").click(function(){
						var _this = this;
						var flag = this.dataset.flag;
						this.innerHTML = '';
						var url = admin.widget.createDom("input",null,this);
						$(url).focus();

						$(url).blur(function(){
							_this.style['background-image'] = "url("+url.value+")";
							$(this).remove();
						});

						admin.widget.registEnterKeyEvent(url,function(){
							_this.style['background-image'] = "url("+url.value+")";
							var img = new Image();
							img.src = url.value;
							img.onload = function(){
								var o = {};
								o.Url = this.src;
								o.Width = this.width;
								o.Height = this.height;
								o.Type = 0;
								self.TmpData.Images[flag] = o;
								$(url).remove();
								console.log(self.TmpData.Images);
							}
						})

					});
				},
				clear:function(){
					ctl.clearText();	
					ctl.clearRadio();
				},
				submit:function(){
					$.ajax({
						type:'post',
						//url:"http://47.104.84.30/api/products",
						url:"/api/products",
						datatype:'json',
						data:self.TmpData,
						contentType:"application/json; charset=UTF-8",
						success:function(data){
							console.log(data);
						},
						error:function(msg){
							console.log(msg);
						}
					})
					console.log(self.TmpData);
					//$.post("/api/products",self.TmpData,function(data){
					//});
				},
				setForm:function(){
				
				},
				getForm:function(){
				
				}
			}
			ctl.init();
		}
	}
	
	admin.NTGoodsEditor = new NTGoodsEditor();
})();


$(document).ready(function(){
	$(".panel").css("height",admin.widget.getWClientHeight());
	$(".g-info-panel").css("height",admin.widget.getWClientHeight()-75);

	$(".tool-btn i").click(function(){
		$(".ctl-panel").hide();
		$(".content-page").hide();
		$("#"+this.dataset.flag+"Panel").show();
		$("#"+this.dataset.flag+"Page").show();



		admin.curPage = this.dataset.flag;

		$(".tool-btn i").removeClass("b-on").addClass("b-off");
		$(this).removeClass("b-off").addClass("b-on");
	});
	$(".b-on").click();


	admin.IMGEditor.init();
	admin.NTGoodsEditor.init();
});
