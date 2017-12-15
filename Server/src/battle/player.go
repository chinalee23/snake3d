package battle

import (
	"fmt"
	"network/message"
	"network/netdef"
)

type stPlayer struct {
	id     int
	conn   netdef.Connection
	chMsg  chan *message.Message
	status string
	room   *stRoom
	chexit chan bool
}

func newPlayer(id int, conn netdef.Connection, room *stRoom) *stPlayer {
	return &stPlayer{
		id:     id,
		conn:   conn,
		chMsg:  make(chan *message.Message),
		status: "idle",
		room:   room,
		chexit: make(chan bool),
	}
}

func (p *stPlayer) start() {
	p.conn.Start(p.chMsg)
	go func() {
		for {
			select {
			case <-p.chexit:
				return
			case msg := <-p.chMsg:
				switch p.status {
				case "idle":
					if msg.MsgType == _enterReq {
						fmt.Println("...........receive enter")
						p.room.onEnter(p)
					}
				case "entered":
					if msg.MsgType == _roomstartReq {
						p.room.onRoomStart()
					}
				case "waitReady":
					if msg.MsgType == _ready {
						p.room.onReady(p)
					}
				case "fight":
					if msg.MsgType == _frame {
						p.room.onFrame(p, msg.Data)
					}
				}
			default:
				break
			}
		}
	}()
}

func (p *stPlayer) onEnter() {
	fmt.Println("player enter")
	// pb := &sgio_battle.Enter{
	// 	Playerid: proto.Int(p.id),
	// 	Roomid:   proto.Int(p.room.roomid),
	// }
	// data, err := proto.Marshal(pb)
	// if err != nil {
	// 	fmt.Println("marshal err:", err)
	// }
	// p.conn.Write(int(sgio_battle.MsgType_enter), data)
	p.status = "entered"
}

func (p *stPlayer) close() {
	p.conn.Close()
	p.chexit <- true
}

func (p *stPlayer) send(msgType int, data []byte) {
	p.conn.Write(msgType, data)
}
