mergeInto(LibraryManager.library, {
    SendPostMessage: function(messagePtr) {
      var message = UTF8ToString(messagePtr);
      console.log('SendReactPostMessage, message sent: ' + message);
      if(window.parent.dispatchReactUnityEvent != null){
        window.parent.dispatchReactUnityEvent(message);
      }
    }
});
