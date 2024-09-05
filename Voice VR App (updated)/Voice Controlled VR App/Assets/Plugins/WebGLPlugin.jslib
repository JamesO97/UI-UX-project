//for testing purposes so far. Might need clean-up
var WebGLFunctions =
{

	/*this is added so that startVoiceRec can use reStartVoiceRec*/
	startVoiceRec__deps:['reStartVoiceRec'],

	/*
	startVoiceRec
	function starts voice recognition using WebSpeech API. API should be supported by
	most modern browsers. Function takes the result of the api and calls function PrintToScene
	to pass the value to c# code. The 'webkitSpeechRecognition' object is being assigned to
	'this' to make it a persistent object accross the different functions.
	WebSpeech API documentation can be found here:
	-https://developers.google.com/web/updates/2013/01/Voice-Driven-Web-Apps-Introduction-to-the-Web-Speech-API
	-https://wicg.github.io/speech-api/#api_description
	*/

	startVoiceRec: function()
	{
		var upgradeInfo = "Voice recognition is not supported by this browser.\n"
		+ "Please upgrade to Chrome verion 25 or later to use recognition";
		if (!('webkitSpeechRecognition' in window))
		{
			console.log(upgradeInfo);
			return;
		};

		this.recognition = new webkitSpeechRecognition();
		var final_transcript = '';
		var lang = ['en-US', 'United States'];
		this.recognition.continuous = true;
		this.recognition.interimResults = true;
		this.recognition.lang = lang;
		this.recognition.start();

		/*
		event handler function. When a result is found, uses send message to trigger the
		"Result" funtion inside WebGLVoiceControl
		*/
		this.recognition.onresult = function(event)
		{
			var interim_transcript = '';
			for (var i = event.resultIndex; i < event.results.length; ++i)
			{
				if (event.results[i].isFinal)
				{
					final_transcript = event.results[i][0].transcript;
					gameInstance.SendMessage('VoiceControlManager', 'Result', final_transcript);
				}
				else
				{
					interim_transcript += event.results[i][0].transcript;
				}
			}
		};

		/*
		These are event handlers used to deal with different potential errors.
		More error cases might need to be added
		*/

		this.recognition.onstart = function(event)
		{
			this.flagNoSpeech = false;
		}

		this.recognition.onerror = function(event)
		{
			console.log(event.error);
			/* if no speech is detected, set flag */
			if (event.error == 'no-speech') {
				this.flagNoSpeech = true;
			}
		};

		this.recognition.onend = function(event)
		{
			/* restart if no speech is detected */
			if (this.flagNoSpeech) {
				console.log('restarting...');
				_reStartVoiceRec();
			}
		};

	},
	/*
	Function called by c# code to stop recogniton on the plugin. It can oly be called if
	"startVoiceRec" has been called before hand.
	*/
	stopVoiceRec: function()
	{
		this.recognition.abort();
	},

	/*
	Function called to restart recogniton on the plugin. It can oly be called if
	"startVoiceRec" has been called before hand.
	*/
	reStartVoiceRec: function()
	{
		this.recognition.start();
	},
}

//this is called by the c# code to use the functions above.
mergeInto(LibraryManager.library, WebGLFunctions);
