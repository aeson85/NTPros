(function(window){
	var NT = {};	
	window.nt = NT;	
})(window);


(function(window){
	var widget = {}; 
	widget.createDom = function(tag,id,attrs,pnode){
		var ts = tag.split(':');
		if(ts[0] == 'img'){
			var dom = new Image();	
		}else{
			var dom = document.createElement(ts[0]);	
		}
		dom.className = ts[1];
		if(id){dom.id = id;}

		for(var i in attrs){
			switch(i){
				case "html":
					dom.innerHTML = attrs[i];break;
				case "type":
					dom.type = attrs[i];break;
				case "value":
					dom.value = attrs[i];break;
				case "src":
					dom.src = attrs[i];break;
				case "width":
					dom.style['width'] = attrs[i];break;
				case "height":
					dom.style['height'] = attrs[i];break;
				case "style":
					for(var j in attrs[i]){dom.style[j]=attrs[i][j];};break;
				//case "click":
					//dom.addEventListener('click',attrs[i],false);break;
				//case "dblclick":
					//dom.addEventListener('dblclick',attrs[i],false);break;
				//case "blur":
					//dom.addEventListener('blur',attrs[i],false);break;
				default:break;
			}
		}
		if(pnode){
			pnode.appendChild(dom);	
		}
		return dom;
	}

	widget.getRect = function(dom){
		return dom.getBoundingClientRect();	
	}
	
	
	widget.waterfall = function(data,config,container,callback,exit,outCon){
		var self = this;
		this.cnum = 0;
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
		this.colsMap = {
			0:0,	
			1:0
		};
		this.colsPot = 0;

		this.init = function(){
			this.initData();
			this.sTop = 0;
			window.onresize = self.resize;
		}

		this.resize = function(){
			if(cosmic.waterfallLock){return;}
			self.sTop = $(window).scrollTop();
			self.isResize = true;
			self.createColumn(function(){
				self.data = self.curData;
				self.createItems(true,self.curData);
			});
		}

		this.getNext = function(h,exit){
			self.exit = exit;
			self.count+= self.lCount;
			self.curData = self.curData.concat(self.data);
			self.createItems(true,self.data);
		}

		this.initData = function(){
			var count = self.count;
			self.count+= self.lCount;
			self.curData = self.data;
			self.createColumn(function(){
				self.createItems(true,self.data);
			});

		}

		this.getCurColumn = function(callback){
			h1 = self.colsMap[0];
			h2 = self.colsMap[1];
			self.colsPot = (h1>h2)?1:0;
			var c = (h1>h2)?self.cols[1]:self.cols[0];
			callback(c);
		}

		this.createColumn = function(callback){
			//var clen = Math.floor(($(window).width()-self.offset)/(self.cWidth+self.gutter));
			//if(clen<4){clen = 4;}
			clen = 2;
			//if(self.clen && self.clen == clen){return;}
			//self.clen = clen;
			container.innerHTML = '';	
			self.cols = [];
			for(var i=0;i<clen;i++){
				var column = widget.createDom('div:waterfall-column','col_'+i,null,container);
				//column.style['width'] = self.cWidth + self.gutter + 'px';
				//column.style['float'] = 'left';
				//column.style['overflow'] = 'hidden';
				self.cols.push(column);
			}

			if(outCon){
				//outCon.style['width'] = (self.cWidth+self.gutter)*clen + 'px';
			}
			//container.style['width'] = (self.cWidth+self.gutter)*clen + 'px';
			var clear = widget.createDom('div',null,null,container);
			clear.style['clear'] = 'both';
			callback();
		}

		this.createItems = function(flag,data){
			self.items = [];
			if(flag){
				var len = data.length,i=0;
				var load = function(h){
					self.colsMap[self.colsPot]+=h;
					var cname = self.config.itemSelector;
					if(i>len-1){
						//if(self.exit){ 
							////if(self.sTop && self.isResize){
								////$(window).scrollTop(self.sTop);
								////self.isResize = false;
							////}
							//self.exit();
						//}
						return;
					}
					self.getCurColumn(function(layer){
						var item = widget.createDom('div:'+cname,null,null,layer);
						self.items.push(item);
						callback(self,item,i++,load);
					});
				}
				load(0);
				return;
			}

			//$(data).each(function(i){
				//var cname = self.config.itemSelector;
				//self.getCurColumn(function(layer){
					//var item = widget.createDom('div:'+cname,null,null,layer);
					//callback(self,item,i);
				//});
			//});
			//if(exit){
				//exit();	
			//}
		}
		this.init();
	}
	nt.widget = widget;
})(window);
