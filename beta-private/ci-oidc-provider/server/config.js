var a2_0x51d1=['client_secret_basic','ADS_CLIENT','Password1!','id_token\x20token','implicit','sub','email','name','groups','exports','CONFIG_FILE','readFileSync','defaults','lodash','IDP_NAME','env','PORT','bar','REDIRECTS','http://localhost:9055/v1/authenticate/callback','apiclient-secret','client_credentials','https://devnull/ignored'];(function(_0x500366,_0x52277a){var _0x3340fc=function(_0x26869b){while(--_0x26869b){_0x500366['push'](_0x500366['shift']());}};_0x3340fc(++_0x52277a);}(a2_0x51d1,0x138));var a2_0x4fcd=function(_0x5ac677,_0x1d9df0){_0x5ac677=_0x5ac677-0x0;var _0x1b2eca=a2_0x51d1[_0x5ac677];return _0x1b2eca;};const _=require(a2_0x4fcd('0x0'));const fs=require('fs');let defaultConfig={'idp_name':process['env'][a2_0x4fcd('0x1')]||'http://simple-oidc-provider','port':process[a2_0x4fcd('0x2')][a2_0x4fcd('0x3')]||0x2328,'client_config':[{'client_id':'foo','client_secret':a2_0x4fcd('0x4'),'redirect_uris':process[a2_0x4fcd('0x2')][a2_0x4fcd('0x5')]&&process[a2_0x4fcd('0x2')][a2_0x4fcd('0x5')]['split'](',')||[a2_0x4fcd('0x6')]},{'client_id':'apiclient','client_secret':a2_0x4fcd('0x7'),'grant_types':[a2_0x4fcd('0x8'),'authorization_code'],'redirect_uris':[a2_0x4fcd('0x9')],'token_endpoint_auth_method':a2_0x4fcd('0xa')},{'client_id':a2_0x4fcd('0xb'),'client_secret':a2_0x4fcd('0xc'),'redirect_uris':[a2_0x4fcd('0x9')],'response_types':[a2_0x4fcd('0xd')],'grant_types':[a2_0x4fcd('0x8'),a2_0x4fcd('0xe')],'token_endpoint_auth_method':'client_secret_basic'}],'claim_mapping':{'openid':[a2_0x4fcd('0xf')],'email':[a2_0x4fcd('0x10'),'email_verified'],'profile':[a2_0x4fcd('0x11'),'nickname',a2_0x4fcd('0x12')]}};module[a2_0x4fcd('0x13')]={'getConfig'(){let _0x1453f1;if(process[a2_0x4fcd('0x2')][a2_0x4fcd('0x14')]){let _0x1d7aef=fs[a2_0x4fcd('0x15')](process[a2_0x4fcd('0x2')][a2_0x4fcd('0x14')]);_0x1453f1=_[a2_0x4fcd('0x16')](JSON['parse'](_0x1d7aef['toString']()),defaultConfig);}else{_0x1453f1=defaultConfig;}return _0x1453f1;}};
//# sourceMappingURL=config.js.map