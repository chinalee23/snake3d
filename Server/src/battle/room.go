package battle

import (
	"container/list"
	"encoding/json"
	"fmt"
	"network/netdef"
	"sync"
	"time"
)

type stEnterRsp struct {
	Id int `json:"id"`
}

type stRoomInfo struct {
	Id          int `json:"id"`
	PlayerCount int `json:"playerCount"`
}

type stBattlePlayer struct {
	Id    int     `json:"id"`
	Snake stSnake `json:"snake"`
}

type stBattleInfo struct {
	Players []stBattlePlayer `json:"players"`
}

type stFrames struct {
	Frames []json.RawMessage `json: "frames"`
}

type stRoom struct {
	roomid  int
	nextid  int
	status  string
	players *list.List
	chFrame chan []byte

	mutex  sync.Mutex
	chexit chan bool
}

func newRoom(id int) *stRoom {
	room := &stRoom{
		roomid:  id,
		nextid:  0,
		status:  "wait",
		players: list.New(),
		chFrame: make(chan []byte, 100),
		chexit:  make(chan bool),
	}
	go room.update()
	return room
}

func (p *stRoom) update() {
	for {
		p.mutex.Lock()
		flag := false
		var next *list.Element
		for e := p.players.Front(); e != nil; e = next {
			next = e.Next()
			player := e.Value.(*stPlayer)
			if player.conn.Disconnected() {
				player.close()
				flag = true
				p.players.Remove(e)
			}
		}
		p.mutex.Unlock()
		if flag {
			if p.status == "wait" {
				p.noticeRoomInfo()
			} else {
				if p.players.Len() == 0 {
					fmt.Println("close room")
					p.chexit <- true
					return
				}
			}
		}
		time.Sleep(50 * time.Millisecond)
	}
}

func (p *stRoom) acceptConnection(conn netdef.Connection) {
	playerId := p.nextid
	p.nextid++
	player := newPlayer(playerId, conn, p)
	p.players.PushBack(player)
	player.start()
}

func (p *stRoom) onEnter(player *stPlayer) {
	player.status = "entered"

	enterRsp := &stEnterRsp{
		Id: player.id,
	}
	data, err := json.Marshal(enterRsp)
	if err != nil {
		fmt.Println("marshal enterRsp err:", err)
		return
	}
	player.send(_enterRsp, data)
	fmt.Println("send enterRsp")

	p.noticeRoomInfo()
}

func (p *stRoom) onRoomStart() {
	if p.status != "wait" {
		return
	}
	defer p.mutex.Unlock()
	p.mutex.Lock()

	fmt.Println("rooms start...")

	var next *list.Element
	for e := p.players.Front(); e != nil; e = next {
		next = e.Next()

		player := e.Value.(*stPlayer)
		if player.status != "entered" {
			player.close()
			p.players.Remove(e)
		} else {
			player.status = "waitReady"
		}
	}

	battleInfo := &stBattleInfo{
		Players: make([]stBattlePlayer, p.players.Len()),
	}
	for e := p.players.Front(); e != nil; e = e.Next() {
		player := e.Value.(*stPlayer)
		battleInfo.Players[player.id] = stBattlePlayer{
			Id:    player.id,
			Snake: battleCfg.Snakes[player.id],
		}
	}
	data, err := json.Marshal(battleInfo)
	if err != nil {
		fmt.Println("marshal battleInfo err:", err)
		return
	}

	p.sendToAllPlayer(_battleStart, data)

	p.status = "start"
}

func (p *stRoom) onReady(player *stPlayer) {
	defer p.mutex.Unlock()
	p.mutex.Lock()

	fmt.Println("onReady...")

	player.status = "fight"
	for e := p.players.Front(); e != nil; e = e.Next() {
		if e.Value.(*stPlayer).status != "fight" {
			return
		}
	}

	p.sendToAllPlayer(_fight, nil)

	fmt.Println("fight")
	go p.fight()
}

func (p *stRoom) onFrame(player *stPlayer, data []byte) {
	defer p.mutex.Unlock()
	p.mutex.Lock()
	fmt.Println("receive frame", string(data))
	p.chFrame <- data
}

func (p *stRoom) noticeRoomInfo() {
	roomInfo := &stRoomInfo{
		Id:          p.roomid,
		PlayerCount: p.players.Len(),
	}
	data, err := json.Marshal(roomInfo)
	if err != nil {
		fmt.Println("marshal roomInfo err:", err)
		return
	}
	p.sendToAllPlayer(_roominfo, data)
}

func (p *stRoom) fight() {
	for {
		select {
		case <-p.chexit:
			return
		default:
			break
		}
		time.Sleep(50 * time.Millisecond)
		frames := p.getFrames()
		jsFrames := &stFrames{
			Frames: frames,
		}
		data, err := json.Marshal(jsFrames)
		if err != nil {
			fmt.Println("marshal frames err:", err)
			continue
		}
		fmt.Println(string(data))
		p.sendToAllPlayer(_frame, data)
	}
}

func (p *stRoom) getFrames() []json.RawMessage {
	defer p.mutex.Unlock()
	p.mutex.Lock()

	frames := make([]json.RawMessage, 0)
	for {
		select {
		case frame := <-p.chFrame:
			frames = append(frames, frame)
		default:
			return frames
		}
	}
}

func (p *stRoom) sendToAllPlayer(msgType int, data []byte) {
	for e := p.players.Front(); e != nil; e = e.Next() {
		e.Value.(*stPlayer).send(msgType, data)
	}
}
