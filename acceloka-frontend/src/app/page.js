import TicketList from "../components/TicketList";

export default function Home() {
  return (
    <div className="container mx-auto p-6">
      <h1 className="text-2xl font-bold text-center mb-6">Acceloka Tickets</h1>
      <TicketList />
    </div>
  );
}
