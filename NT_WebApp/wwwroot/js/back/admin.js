(function(window){
	var admin = {};
	admin.debug = false;
	admin.path = admin.debug?"http://47.104.84.30":""
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

		this.getRect = function(dom){
			return dom.getBoundingClientRect();	
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
	
		this.request = function(url,data,callback,type,contentType){
			var requestWorker = new Worker('../../js/back/request.js');
			var ctype;
			requestWorker.postMessage({url:url,data:data,type:type,contentType:contentType});
			requestWorker.onmessage = function(e) {
				if(e.data.StatusCode == 500){
					cosmic.error();
				}
				if(e.data.StatusCode == 1004){
					return;
				}
				callback(e.data);
				requestWorker.terminate();
			};
		}

	
		this.waterfall = function(data,config,container,callback,exit,outCon){	
			var self = this;
			this.exit = exit;
			this.data = data;	
			this.config = config;
			if(outCon){
				this.outCon = outCon;
			}
			this.container = container;
			this.offset = config.offset;
			this.cWidth = config.columnWidth;
			this.gutter = config.gutter;
			this.lCount = config.loadCount;
			this.count = 0;
			this.curData = [];
			this.cols = [];
			this.clen = 0;
			//this.scrollLock = false;

			this.init = function(){
				this.initData();
				this.sTop = 0;
				window.onresize = self.resize;
			}

			this.resize = function(){
				//if(cosmic.waterfallLock){return;}
				self.sTop = $(window).scrollTop();
				self.isResize = true;
				self.createColumn(function(){
					self.data = self.curData;
					self.createItems(true,self.curData);
				});
			}

			this.getNext = function(exit){
				self.exit = exit;
				self.count+= self.lCount;
				self.curData = self.curData.concat(self.data);
				self.createItems(true,self.data);
			}

			this.initData = function(){
				var count = self.count;
				self.count+= self.lCount;
				//self.curData = self.data.slice(count,self.count);
				self.curData = self.data;
				self.createColumn(function(){
					self.createItems(true,self.data);
				});

			}

			this.getCurColumn = function(callback){
				var _c = null;
				for(var i=0;i<self.cols.length;i++){
					if(_c){
						_c.onload = function(){
						}
						var h1 = admin.widget.getRect(_c).height;
						var h2 = admin.widget.getRect(self.cols[i]).height;
						_c = (h2>=h1)?_c:self.cols[i];
						continue;	
					}
					_c = self.cols[i];
				}
				callback(_c);
			}

			this.createColumn = function(callback){
				var clen = Math.floor(($(container).width()-self.offset)/(self.cWidth+self.gutter));
				if(clen<2){clen = 2;}
				if(self.clen && self.clen == clen){return;}
				self.clen = clen;
				container.innerHTML = '';	
				self.cols = [];
				for(var i=0;i<clen;i++){
					var column = admin.widget.createDom('div',null,container);
					column.style['width'] = self.cWidth + self.gutter + 8 + 'px';
					column.style['padding'] = '10px 0';
					column.style['float'] = 'left';
					column.style['overflow'] = 'hidden';
					console.log(container);
					self.cols.push(column);
				}

				if(outCon){
					outCon.style['width'] = (self.cWidth+self.gutter)*clen + 'px';
				}
				//container.style['width'] = (self.cWidth+self.gutter)*clen + 'px';
				//$cm.ID('masonry').style['width'] = (self.cWidth+self.gutter)*clen + 'px';
				var clear = admin.widget.createDom('div',null,null,container);
				clear.style['clear'] = 'both';
				callback();
			}

			this.createItems = function(flag,data){
				self.items = [];
				if(flag){
					var len = data.length,i=0;
					var load = function(){
						var cname = self.config.itemSelector;
						if(i>len-1){
							if(self.exit){ 
								if(self.sTop && self.isResize){
									$(window).scrollTop(self.sTop);
									self.isResize = false;
								}
								self.exit();
							}
							return;
						}
						self.getCurColumn(function(layer){
							var item = admin.widget.createDom('div:'+cname,'col_'+i,layer);
							item.style['width'] = self.cWidth + 'px';
							self.items.push(item);
							callback(self,item,i++,load);
						});
					}
					load();
					return;
				}

				$(data).each(function(i){
					var cname = self.config.itemSelector;
					self.getCurColumn(function(layer){
						var item = admin.widget.createDom('div:'+cname,null,layer);
						item.style['width'] = self.cWidth + 'px';
						callback(self,item,i);
					});
				});
				if(exit){
					exit();	
				}
			}
			this.init();
		}

	}

	admin.widget = new widget();	
})();


(function(window){
	var Manager = function(){
		var self = this;
		this.init = function(){
			this.initFrameSize();
			this.initEvent();
			this.initShortcut();	

			$(window).resize(this.initFrameSize);
		}	

		this.initFrameSize = function(){
			$(".panel").css("height",admin.widget.getWClientHeight());
			$(".g-info-panel").css("height",admin.widget.getWClientHeight()-75);
		}

		this.initEvent = function(){
			self.navTarget = function(n){
				$(".tool-btn i").eq(n).click();
			}

			$(".tool-btn i").click(function(){
				$(".ctl-panel").hide();
				$(".content-page").hide();
				$("#"+this.dataset.flag+"Panel").show();
				$("#"+this.dataset.flag+"Page").show();
				admin.curPage = this.dataset.flag;
				$(".tool-btn i").removeClass("b-on").addClass("b-off");
				$(this).removeClass("b-off").addClass("b-on");
			});

			self.navTarget(1);
		}

		this.initShortcut = function(){
			document.body.onkeydown = function(e){
				console.log(e.keyCode);
				if(e.keyCode == '49' && e.altKey){
					self.navTarget(1);
				}

				if(e.keyCode == '50' && e.altKey){
					self.navTarget(2);
				}
			}
		}
	}

	admin.manager = new Manager();
})(window);


(function(window){
	var IMGEditor = function(){
		var self = this;
		var menuMap = {
			"AllIMG":"所有图片","OrderIMG":"点餐区","Activity":"分享会","Breakfast":"早餐","Package":"套餐活动","Salad":"沙拉，健身餐","ShelfGoods":"货架商品","Coffee":"咖啡","Drinks":"其他饮品"}
		this.init = function(){
			this.initMenuTree();	
		}

		this.initMenuTree = function(){
			$.get(admin.path + "/api/ftp/",null,function(data){
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
			container.innerHTML = "";
			$.get(admin.path+"/api/ftp/"+this.dataset.name,null,function(data){
				$(data).each(function(i){
					var viewBox = admin.widget.createDom("div:img-view-box",null,container);
					var view = admin.widget.createDom("div:img-view",null,viewBox);
					var name = admin.widget.createDom("div:img-view-name",null,viewBox);
					name.innerHTML = this.name;

					var imgUrl = admin.widget.createDom("div:img-view-url",null,viewBox);
					var url = admin.path+this.url;
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


				});
			});
		}
	}	
	admin.IMGEditor = new IMGEditor();
})(window);


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
			var _this = this;
			this.ctl = {
				type:0,
				init:function(){
					this.registEvent();	
					this.registTextEvent();
					this.registRadioEvent();
					this.registTextareaEvent();
				},
				registTextEvent:function(){
					$(_table).find("input[type='text']").each(function(){
						$(this).keyup(function(){
							if(this.dataset.arr){
								self.TmpData[this.dataset.arr][this.dataset.flag] = this.value;
							}else{
								self.TmpData[this.dataset.flag] = this.value;
							}
						});
					});
				},
				setText:function(arr,flag,val){
					if(arr){
						$(_table).find("input[type='text'][data-arr="+arr+"][data-flag="+flag+"]").val(val);
						self.TmpData[arr][flag] = val;
					}else{
						$(_table).find("input[type='text'][data-flag="+flag+"]").val(val);
						self.TmpData[flag] = val;
					}
				},
				clearText:function(){
					$(_table).find("input[type='text']").each(function(){
						this.value = "";	
						if(this.dataset.arr){
							self.TmpData[this.dataset.arr][this.dataset.flag] = 0;
						}else{
							self.TmpData[this.dataset.flag] = this.value;
						}
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
				setRadio:function(flag,val){
					self.TmpData[flag] = val;
					$(_table).find("input[type='radio']").each(function(){
						if(this.value == val){
							this.checked = true;	
						}else{
							this.checked = false;	
						}
					})
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
				setTextarea:function(flag,val){
					console.log(flag,val)
					$(_table).find("div[contenteditable='true'][data-flag="+flag+"]").html(val);
					self.TmpData[flag] = val;
				},
				clearTextarea:function(){
					$(_table).find("div[contenteditable='true']").each(function(){
						this.innerHTML = "";	
						self.TmpData[this.dataset.flag]
					});
				},
				setImage:function(n,data){
					$(".g-view").eq(n)[0].style["background-image"] = "url("+data.url+")";
					self.TmpData.Images[n].Url = data.url;
					self.TmpData.Images[n].Width = data.width;
					self.TmpData.Images[n].Height = data.height;
					self.TmpData.Images[n].Type = data.type;
				},
				clearImage:function(){
					$(".g-view").each(function(){
						this.style["background-image"] = "none";
						$(self.Images).each(function(){
							this.Url = "";
							this.Width = 0;
							this.Height = 0;
							this.Type = 0;
						});
					})
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
							//$(this).remove();
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
					_this.ctl.type = 0;	
					$("#submitGEdit")[0].className = "fa fa-plus";
					self.TmpData.Id = null;
					_this.ctl.clearText();	
					_this.ctl.clearTextarea();	
					_this.ctl.clearRadio();
					_this.ctl.clearImage();
				},
				submit:function(){
					console.log(self.TmpData);
					$.ajax({
						type:_this.ctl.type?'put':'post',
					url:admin.path+"/api/products",
					datatype:'json',
					data:JSON.stringify(self.TmpData),
					headers:{
						"Access-Control-Allow-Headers":"Origin, Content-Type, Cookie, Accept",
					"Content-Type" : "application/json; charset=UTF-8",
					"X-Requested-With":"XMLHttpRequest"
					},
					success:function(data){
						admin.GoodShelves.init();
						$("#submitGEdit")[0].className = "fa fa-plus";
						_this.ctl.clear();
						_this.ctl.type = 0;	
					},
					error:function(msg){
						console.log(msg);
					}
					});
				},
				setForm:function(data){
					_this.ctl.type = 1;	
					self.TmpData.Id = data.id;
					$("#submitGEdit")[0].className = "fa fa-repeat";
					_this.ctl.setText("Prices","Original",data.prices.original);
					_this.ctl.setText("Prices","Membership",data.prices.membership);
					_this.ctl.setText("Prices","Present",data.prices.present);
					_this.ctl.setText(null,"Name",data.name);
					_this.ctl.setText(null,"Title",data.title);
					_this.ctl.setTextarea("Introduction",data.introduction);
					_this.ctl.setTextarea("Type",data.type);
					_this.ctl.setRadio("Group",data.group);
					_this.ctl.setRadio("PubType",data.pubType);
					_this.ctl.setRadio("CanCollection",data.canCollection);
					_this.ctl.setImage(0,data.images[0]);
					_this.ctl.setImage(1,data.images[1]);
					_this.ctl.setImage(2,data.images[2]);
				},
				getForm:function(){

				}
			}
			this.ctl.init();
		}
	}

	var GoodShelves = function(){
		var self = this;
		this.init = function(){
			this.getRequestData();	
		}	

		this.getRequestData = function(){
			$.get(admin.path+"/api/products",null,function(data){
				console.log(data);
				self.initWaterfall(data);
			});
		}

		this.initWaterfall = function(data){
			var container = $('#goodsPage')[0];
			container.innerHTML = "";
			var configure = {
				offset:0,
				columnWidth:220,
				gutter:10,
				itemSelector:'ele-item'
			};

			var waterfall = new admin.widget.waterfall(data,configure,container,function(entity,item,i,next){
				self.createItem(item,entity.data[i],function(h){
					next(h);
				});
			},null,null);
		}

		this.createItem = function(item,data,next){
			var eleBG = admin.widget.createDom("div:ele-bg",null,item);
			eleBG.style['background-image'] = "url("+data.images[0].url+")";
			var usrBar = admin.widget.createDom("div:usr-bar",null,item); 
			var usrBarBG = admin.widget.createDom("div:usr-bar-bg",null,usrBar); 
			var name = admin.widget.createDom("p",null,usrBar); 
			name.innerHTML = data.name;
			var op= admin.widget.createDom("h2",null,usrBar); 
			op.innerHTML ="原价：￥"+data.prices.original;
			var pp = admin.widget.createDom("span",null,op); 
			pp.innerHTML ="现价：￥"+ data.prices.present;
			var mp = admin.widget.createDom("span",null,op); 
			mp.innerHTML ="会员价：￥"+ data.prices.membership;
			//var ico = admin.widget.createDom("a:fa fa-bars usr-mark-ico-right",null,null,usrBar); 
			//$(ico).attr("aria-hidden","true");
			

			var sw = data.images[0].width;	
			var sh = data.images[0].height;
			var w = 220; 
			var h =  w/sw*sh;
			h = h<w?w:h;
			eleBG.style["width"] = "100%";
			eleBG.style["height"] = h + "px";
			next(h);

			//eleBG.style["width"] = "100%";
			//eleBG.style["height"] = h + "px";

			//var img = new Image();
			//img.src = data.images[0].url;
			//var sw,sh;
			//img.onload = function(){
				//sw = this.width;	
				//sh = this.height;
				//var w = 220; 
				//var h =  w/sw*sh;
				//h = h<w?w:h;

				//eleBG.style["width"] = "100%";
				//eleBG.style["height"] = h + "px";
				//next(h);	
			//}
			//
			$(item).click(function(){
				admin.NTGoodsEditor.ctl.setForm(data);
			})
		}

	}

	admin.NTGoodsEditor = new NTGoodsEditor();
	admin.GoodShelves = new GoodShelves();
	admin.GoodShelves.init();
})();


$(document).ready(function(){
	admin.manager.init();
	admin.IMGEditor.init();
	admin.NTGoodsEditor.init();
});
