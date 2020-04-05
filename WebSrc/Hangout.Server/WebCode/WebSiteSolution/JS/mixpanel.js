var MixpanelLib=function(q,r){metrics={};metrics.super_properties={"all":{},"events":{},"funnels":{}};metrics.send_request=function(a,b){var c=metrics.callback_fn;if(a.indexOf("?")>-1){a+="&callback="}else{a+="?callback="}a+=c+"&";if(b){a+=metrics.http_build_query(b)}a+='&_='+new Date().getTime().toString();var d=document.createElement("script");d.setAttribute("src",a);d.setAttribute("type","text/javascript");document.body.appendChild(d)};metrics.log=function(a,b){if(!a.project){a.project=metrics.token}if(a.project&&a.category){metrics.callback=b;a.ip=1;metrics.send_request(metrics.api_host+"/log/",a)}};metrics.track_funnel=function(a,b,c,d,e){if(!d){d={}}d.funnel=a;d.step=parseInt(b,10);d.goal=c;if(d.step==1){if(document.referrer.search('http://(.*)google.com')===0){var f=metrics.get_query_param(document.referrer,'q');if(f.length){metrics.register({'mp_keyword':f},'funnels')}}}metrics.track('mp_funnel',d,e,"funnels")};metrics.get_query_param=function(a,b){b=b.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");var c="[\\?&]"+b+"=([^&#]*)";var d=new RegExp(c);var e=d.exec(a);if(e===null||(e&&typeof(e[1])!='string'&&e[1].length)){return''}else{return unescape(e[1]).replace(/\+/g,' ')}};metrics.track=function(a,b,c,d){if(!d){d="events"}if(!b){b={}}if(!b.token){b.token=metrics.token}if(c){metrics.callback=c}b.time=metrics.get_unixtime();if(d!="all"){for(var p in metrics.super_properties[d]){if(!b[p]){b[p]=metrics.super_properties[d][p]}}}if(metrics.super_properties.all){for(p in metrics.super_properties.all){if(!b[p]){b[p]=metrics.super_properties.all[p]}}}var e={'event':a,'properties':b};var f=metrics.base64_encode(metrics.json_encode(e));metrics.send_request(metrics.api_host+'/track/',{'data':f,'ip':1})};metrics.register_once=function(a,b,c,d){if(!b||!metrics.super_properties[b]){b="all"}if(!c){c="None"}if(!d){d=7}if(a){for(var p in a){if(p){if(!metrics.super_properties[b][p]||metrics.super_properties[b][p]==c){metrics.super_properties[b][p]=a[p]}}}}metrics.set_cookie("mp_super_properties",metrics.json_encode(metrics.super_properties),d)};metrics.register=function(a,b,c){if(!b||!metrics.super_properties[b]){b="all"}if(!c){c=7}if(a){for(var p in a){if(p){metrics.super_properties[b][p]=a[p]}}}metrics.set_cookie("mp_super_properties",metrics.json_encode(metrics.super_properties),c)};metrics.http_build_query=function(a,b){var c,use_val,use_key,i=0,tmp_arr=[];if(!b){b='&'}for(c in a){if(c){use_val=encodeURIComponent(a[c].toString());use_key=encodeURIComponent(c);tmp_arr[i++]=use_key+'='+use_val}}return tmp_arr.join(b)};metrics.get_unixtime=function(){return parseInt(new Date().getTime().toString().substring(0,10),10)};metrics.jsonp_callback=function(a){if(metrics.callback){metrics.callback(a);metrics.callback=false}};metrics.json_encode=function(j){var l;var m=j;var i;var n=function(b){var d=/[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g;var e={'\b':'\\b','\t':'\\t','\n':'\\n','\f':'\\f','\r':'\\r','"':'\\"','\\':'\\\\'};d.lastIndex=0;return d.test(b)?'"'+b.replace(d,function(a){var c=e[a];return typeof c==='string'?c:'\\u'+('0000'+a.charCodeAt(0).toString(16)).slice(-4)})+'"':'"'+b+'"'};var o=function(a,b){var c='';var d='    ';var i=0;var k='';var v='';var e=0;var f=c;var g=[];var h=b[a];if(h&&typeof h==='object'&&typeof h.toJSON==='function'){h=h.toJSON(a)}switch(typeof h){case'string':return n(h);case'number':return isFinite(h)?String(h):'null';case'boolean':case'null':return String(h);case'object':if(!h){return'null'}c+=d;g=[];if(Object.prototype.toString.apply(h)==='[object Array]'){e=h.length;for(i=0;i<e;i+=1){g[i]=o(i,h)||'null'}v=g.length===0?'[]':c?'[\n'+c+g.join(',\n'+c)+'\n'+f+']':'['+g.join(',')+']';c=f;return v}for(k in h){if(Object.hasOwnProperty.call(h,k)){v=o(k,h);if(v){g.push(n(k)+(c?': ':':')+v)}}}v=g.length===0?'{}':c?'{'+g.join(',')+''+f+'}':'{'+g.join(',')+'}';c=f;return v}};return o('',{'':m})};metrics.base64_encode=function(a){var b="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";var c,o2,o3,h1,h2,h3,h4,bits,i=0,ac=0,enc="",tmp_arr=[];if(!a){return a}a=metrics.utf8_encode(a+'');do{c=a.charCodeAt(i++);o2=a.charCodeAt(i++);o3=a.charCodeAt(i++);bits=c<<16|o2<<8|o3;h1=bits>>18&0x3f;h2=bits>>12&0x3f;h3=bits>>6&0x3f;h4=bits&0x3f;tmp_arr[ac++]=b.charAt(h1)+b.charAt(h2)+b.charAt(h3)+b.charAt(h4)}while(i<a.length);enc=tmp_arr.join('');switch(a.length%3){case 1:enc=enc.slice(0,-2)+'==';break;case 2:enc=enc.slice(0,-1)+'=';break}return enc};metrics.utf8_encode=function(a){a=(a+'').replace(/\r\n/g,"\n").replace(/\r/g,"\n");var b="";var c,end;var d=0;c=end=0;d=a.length;for(var n=0;n<d;n++){var e=a.charCodeAt(n);var f=null;if(e<128){end++}else if((e>127)&&(e<2048)){f=String.fromCharCode((e>>6)|192)+String.fromCharCode((e&63)|128)}else{f=String.fromCharCode((e>>12)|224)+String.fromCharCode(((e>>6)&63)|128)+String.fromCharCode((e&63)|128)}if(f!==null){if(end>c){b+=a.substring(c,end)}b+=f;c=end=n+1}}if(end>c){b+=a.substring(c,a.length)}return b};metrics.set_cookie=function(a,b,c){var d=new Date();d.setDate(d.getDate()+c);document.cookie=a+"="+escape(b)+((c===null)?"":";expires="+d.toGMTString())+"; path=/"};metrics.get_cookie=function(a){if(document.cookie.length>0){var b=document.cookie.indexOf(a+"=");if(b!=-1){b=b+a.length+1;var c=document.cookie.indexOf(";",b);if(c==-1){c=document.cookie.length}return unescape(document.cookie.substring(b,c))}}return""};metrics.get_super=function(){var a=eval('('+metrics.get_cookie("mp_super_properties")+')');if(a){for(var i in a){if(i){metrics.super_properties[i]=a[i]}}}};var s=(("https:"==document.location.protocol)?"https://":"http://");metrics.token=q;metrics.api_host=s+'api.mixpanel.com';if(r){metrics.callback_fn=r+'.jsonp_callback'}else{metrics.callback_fn='mpmetrics.jsonp_callback'}try{metrics.get_super()}catch(err){}return metrics};