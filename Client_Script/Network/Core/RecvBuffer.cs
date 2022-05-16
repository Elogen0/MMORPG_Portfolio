using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    //패킷을 일부분만 처리할수 있는 기능
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos; // 시작: 0 패킷이 다 왔는지 검사해서 읽을 커서
        int _writePos; // 시작: 0 받은 바이트를 메모리에 쓰는 커서
        //아이디어: 커서가 뒤로 움직이면서 읽고쓰는 작업을하고 [][][][r][w] 하나의 완성된 패킷을 받으면 그만큼 커서를 앞으로 민다 [r][w][][][]
        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos - _readPos; } } //데이터가 채워지공간 [r]~[w]
        public int FreeSize { get { return _buffer.Count - _writePos; } } //남는공간 [w] ~

        public ArraySegment<byte> DataSegment //어디부터 읽으면 되는가? [r][][]/[w]
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment //리시블 할 수 있는 남는공간
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                //남은 데이터가 없으면 복사지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // r~w 사이에 남은 찌끄레기가 있으면 같이 끌고가서 시작 위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        /// <summary>
        /// 컨텐츠코드를 성공적으로 처리하면 불려짐
        /// </summary>
        /// <param name="numOfByte">받은 바이트 크기</param>
        /// <returns></returns>
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }

        /// <summary>
        /// 클라에서 쏴주는 데이터를 받았을 때
        /// </summary>
        /// <param name="numOfByte">받은 바이트 크기</param>
        /// <returns></returns>
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }
    }
}
