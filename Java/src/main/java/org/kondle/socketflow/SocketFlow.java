package org.kondle.socketflow;

import java.net.Socket;

public class SocketFlow
{
    private static int initListenersCount = 5;
    private static int listenersCountExtendSize = 5;


    private int connectListenersRealCnt = 0;
    private int messageListenersRealCnt = 0;
    private int disconnectListenersRealCnt = 0;

    private SocketFlowConnectListener[] connectListeners;
    private SocketFlowMessageListener[] messageListeners;
    private SocketFlowDisconnectListener[] disconnectListeners;

    private Socket socket;
    public SocketFlow(Socket socket)
    {
        this.socket = socket;

        connectListeners = new SocketFlowConnectListener[initListenersCount];
        messageListeners = new SocketFlowMessageListener[initListenersCount];
        disconnectListeners = new SocketFlowDisconnectListener[initListenersCount];
    }

    void addConnectListener(SocketFlowConnectListener listener)
    {
        this.connectListeners[connectListenersRealCnt] = listener;

        if (connectListenersRealCnt + 1 == connectListeners.length)
        {
            SocketFlowConnectListener[] newListenersArr = new SocketFlowConnectListener[connectListenersRealCnt + 1 + listenersCountExtendSize];
            System.arraycopy(connectListeners,0,newListenersArr,0,connectListenersRealCnt + 1);
            this.connectListeners = newListenersArr;
        }

        connectListenersRealCnt++;
    }

    void addMessageListener(SocketFlowMessageListener listener)
    {
        this.messageListeners[messageListenersRealCnt] = listener;

        if (messageListenersRealCnt + 1 == messageListeners.length)
        {
            SocketFlowMessageListener[] newListenersArr = new SocketFlowMessageListener[messageListenersRealCnt + 1 + listenersCountExtendSize];
            System.arraycopy(messageListeners,0,newListenersArr,0,messageListenersRealCnt + 1);
            this.messageListeners = newListenersArr;
        }


        messageListenersRealCnt++;
    }

    void addDisconnectListener(SocketFlowDisconnectListener listener)
    {
        this.disconnectListeners[disconnectListenersRealCnt] = listener;
        if (disconnectListenersRealCnt + 1 == disconnectListeners.length)
        {
            SocketFlowDisconnectListener[] newListenersArr = new SocketFlowDisconnectListener[messageListenersRealCnt + 1 + listenersCountExtendSize];
            System.arraycopy(disconnectListeners,0,newListenersArr,0,messageListenersRealCnt + 1);
            this.disconnectListeners = newListenersArr;
        }
    }


}
