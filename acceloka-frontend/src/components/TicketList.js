"use client";
import { toast } from "react-hot-toast";
import { Toaster } from "react-hot-toast";
import { useEffect, useState } from "react";
import axios from "axios";
import { motion, AnimatePresence } from "framer-motion";

const API_BASE_URL = "http://localhost:5297/api/v1"; // Adjust if necessary

export default function TicketList() {
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [bookingData, setBookingData] = useState({});
  const [message, setMessage] = useState(""); // Store success/error messages
  const [bookedTickets, setBookedTickets] = useState([]); // Stores booked tickets
  const [guid, setGuid] = useState(""); // Stores user input GUID
  const [bookedTicketId, setBookedTicketId] = useState(null); // Stores the GUID
  const [editData, setEditData] = useState({});

  const [searchQuery, setSearchQuery] = useState(""); // Search filter state
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [minPrice, setMinPrice] = useState("");
  const [maxPrice, setMaxPrice] = useState("");

  useEffect(() => {
    console.log("📢 Loaded Booked Tickets Data:", bookedTickets);
  }, [bookedTickets]);

    useEffect(() => {
      axios
        .get(`${API_BASE_URL}/get-available-ticket`)
        .then((response) => {
          if (Array.isArray(response.data.items)) {
            setTickets(response.data.items);
          } else {
            console.error("Expected an array but got:", response.data);
            setTickets([]);
          }
        })
        .catch((error) => {
          console.error("Error fetching tickets:", error);
          setError("Failed to load tickets. Please try again later.");
        })
        .finally(() => {
          setLoading(false);
        });
    }, []);

    const filteredTickets = tickets.filter((ticket) => {
      const ticketDate = new Date(ticket.eventDate);
      const minPriceValue = minPrice ? parseFloat(minPrice) : 0;
      const maxPriceValue = maxPrice ? parseFloat(maxPrice) : Infinity;
  
      // Apply search filter
      const matchesSearch = `${ticket.name} ${ticket.category} ${ticket.code}`
        .toLowerCase()
        .includes(searchQuery.toLowerCase());
  
      // Apply date filter
      const matchesDate =
        (!startDate || ticketDate >= new Date(startDate)) &&
        (!endDate || ticketDate <= new Date(endDate));
  
      // Apply price filter
      const matchesPrice = ticket.price >= minPriceValue && ticket.price <= maxPriceValue;
  
      return matchesSearch && matchesDate && matchesPrice;
    });

  // Handle input change for ticket quantity
  const handleQuantityChange = (code, value) => {
    setBookingData({ ...bookingData, [code]: value });
  };

  const handleBookTicket = (ticket) => {
    const quantity = bookingData[ticket.code] || 1;
  
    if (quantity < 1) {
      setMessage("❌ Quantity must be at least 1");
      return;
    }
  
    const bookingPayload = {
      Tickets: [
        {
          ticketCode: ticket.code,
          quantity: parseInt(quantity, 10),
        },
      ],
    };
  
    console.log("📌 Booking Payload:", bookingPayload);
  
    axios
      .post(`${API_BASE_URL}/book-ticket`, bookingPayload)
      .then((response) => {
        if (response.data) {
          const bookingId = response.data.bookingId; // Get the GUID
          setMessage(`✅ Successfully booked ${quantity} ticket(s) for ${ticket.name}!`);
          
          // Display toast notification with GUID
          toast.success(`🎉 Booking successful! Your GUID: ${bookingId} `, {
            duration: 6000,
            position: "top-right",
          });
  
          // Update UI: Reduce remaining quota
          setTickets((prevTickets) =>
            prevTickets.map((t) =>
              t.code === ticket.code
                ? { ...t, remainingQuota: t.remainingQuota - quantity }
                : t
            )
          );
        }
      })
      .catch((error) => {
        console.error("Booking error:", error.response?.data || error);
        
        const errorMessage = error.response?.data?.detail || "Check request format";
        setMessage(`❌ Booking failed: ${errorMessage}`);
  
        // Show error notification
        toast.error(`❌ Booking failed: ${errorMessage}`, {
          duration: 5000,
          position: "top-right",
        });
      });
  };

  // Handle fetching booked tickets
  const fetchBookedTickets = () => {
    if (!guid) {
      setMessage("Please enter a valid GUID.");
      return;
    }
    setBookedTicketId(guid);
  
    axios
        .get(`${API_BASE_URL}/booked-tickets`, {
            params: { bookedTicketIds: guid }, // ✅ Ensure correct parameter name
        })
        .then((response) => {
            const bookedData = response.data;
            console.log("📢 Loaded Booked Tickets Data:", bookedData); // ✅ Debugging log
            
            if (bookedData.length === 0) {
                console.warn("⚠ No booked tickets found.");
                setMessage("⚠ No booked tickets found for this GUID.");
                setBookedTickets([]); // Clear the UI if no tickets are found
                return;
              }

            if (bookedData.length > 0) {
            // Try different possible field names for the ID
            const firstBookedTicketId = bookedData[0].bookedTicketId || bookedData[0].id || bookedData[0].ticketId;

            if (firstBookedTicketId) {
                setBookedTicketId(firstBookedTicketId);
                console.log("✅ Stored bookedTicketId:", firstBookedTicketId);
            } else {
                console.warn("⚠ Booked ticket ID is missing in API response.");
            }
            } else {
            console.warn("⚠ No booked tickets found.");
            }

            // Match booked tickets with available ticket details
            const enrichedBookedTickets = bookedData.map((booked) => {
            const fullTicket = tickets.find((t) => t.code === booked.ticketCode);
            return {
                ...booked,
                category: fullTicket?.category || "Unknown",
                price: fullTicket?.price || "N/A",
            };
            });

            setBookedTickets(enrichedBookedTickets);
            setMessage("✅ Successfully fetched booked tickets!");
        })
        .catch((error) => {
            console.error("❌ Error fetching booked tickets:", error);
            setMessage(`❌ Error fetching booked tickets: ${error.response?.data?.detail || "Unknown error"}`);
        });

  };

  useEffect(() => {
    const guid = "328C52C5-52E0-4311-4A66-08DD5A4E9477"; // Example GUID
    fetchBookedTickets(guid); // Fetch details & store GUID
  }, []);

  const revokeTicket = (bookedTicketId, ticketCode, quantity) => {
    if (!bookedTicketId || !ticketCode || quantity <= 0) {
      console.warn("⚠ Missing parameters! Cannot revoke.");
      return;
    }
  
    console.log(`🚀 Revoking ${quantity} tickets for ${ticketCode} with ID: ${bookedTicketId}`);
  
    axios
      .delete(`${API_BASE_URL}/booked-tickets/${bookedTicketId}/${ticketCode}/${quantity}`)
      .then((response) => {
        console.log("✅ Ticket revoked:", response.data);
        setMessage("✅ Successfully revoked ticket!");

        setBookedTickets((prevTickets) =>
            prevTickets
              .map((ticket) =>
                ticket.ticketCode === ticketCode
                  ? { ...ticket, quantity: ticket.quantity - quantity } // Reduce quantity
                  : ticket
              )
              .filter((ticket) => ticket.quantity > 0) // Remove tickets with 0 quantity
          );
      })
      .catch((error) => {
        console.error("❌ Revoke error:", error.response?.data || error);
      });
  };

  const handleEditBookedTicket = (bookedTicketId, ticketCode) => {
    console.log("🛠 DEBUG → Function Called with:");
    console.log("🛠 DEBUG → Booked Ticket ID:", bookedTicketId);
    console.log("🛠 DEBUG → Ticket Code:", ticketCode);

    const newQuantity = parseInt(editData[ticketCode] || 0, 10);
    console.log("🛠 DEBUG → New Quantity:", newQuantity);
  
    if (!bookedTicketId || !ticketCode || newQuantity <= 0) {
      setMessage("❌ Please enter a valid quantity to update.");
      return;
    }

    console.log("🛠 DEBUG → Current Tickets Array:", tickets);
    console.log("🛠 DEBUG → Looking for Ticket Code:", ticketCode);
  
    // ✅ Find the ticket to check the remaining quota
    const ticket = tickets.find((t) => t.code === ticketCode);
    console.log("🛠 DEBUG → Found Ticket:", ticket);
  
    if (!ticket) {
      setMessage("❌ Ticket not found.");
      return;
    }
  
    if (newQuantity > ticket.remainingQuota) {
      setMessage(`❌ Cannot exceed available quota (${ticket.remainingQuota} left).`);
      return;
    }
  
    // ✅ Prepare API request payload
    const payload = [{ KodeTicket: ticketCode, Quantity: newQuantity }];
    console.log("🛠 DEBUG → API Payload:", JSON.stringify(payload, null, 2));
    console.log("🛠 DEBUG → API Endpoint:", `${API_BASE_URL}/booked-tickets/${bookedTicketId}`);

    axios
      .put(`${API_BASE_URL}/booked-tickets/${bookedTicketId}`, payload)
      .then((response) => {
        console.log("✅ Edit success:", response.data);
        setMessage(`✅ Successfully updated ticket quantity!`);
  
        // ✅ Update frontend state after editing
        setBookedTickets((prevTickets) =>
          prevTickets.map((ticket) =>
            ticket.ticketCode === ticketCode
              ? { ...ticket, quantity: newQuantity } // Update quantity
              : ticket
          )
        );
      })
      .catch((error) => {
        console.error("❌ Edit error:", error.response?.data || error);
        setMessage(`❌ Edit failed: ${error.response?.data?.detail || "Unknown error"}`);
      });
  };
  
      

  if (loading) return <p>Loading tickets...</p>;
  if (tickets.length === 0) return <p>No tickets available.</p>;

  return (
    <div className="p-6 max-w-4xl mx-auto">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
        <input
          type="text"
          placeholder="Search by name, category, or code"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="border p-2 w-full text-gray"
        />

        <input
          type="date"
          value={startDate}
          onChange={(e) => setStartDate(e.target.value)}
          className="border p-2 w-full text-gray"
          placeholder="Start Date"
        />
        <input
          type="date"
          value={endDate}
          onChange={(e) => setEndDate(e.target.value)}
          className="border p-2 w-full text-gray"
          placeholder="End Date"
        />

        <input
          type="number"
          placeholder="Min Price"
          value={minPrice}
          onChange={(e) => setMinPrice(e.target.value)}
          className="border p-2 w-full text-gray"
        />
        <input
          type="number"
          placeholder="Max Price"
          value={maxPrice}
          onChange={(e) => setMaxPrice(e.target.value)}
          className="border p-2 w-full text-gray"
        />
      </div>
      {/* Available Tickets Section */}
      <section id="tickets">
        <h2 className="text-2xl font-bold mb-6 text-center text-white">Available Tickets</h2>
      </section>

      {message && (
        <p className="mb-4 text-center text-lg font-semibold text-red-500">
          {message}
        </p>
      )}

      {/* Tickets List */}
      <AnimatePresence mode="wait">
        <motion.div
          key="tickets"
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0, y: -10 }}
          transition={{ duration: 0.5 }}
          className="grid grid-cols-1 md:grid-cols-2 gap-6"
        >
          {filteredTickets.length > 0 ? (
            filteredTickets.map((ticket) => (
              <div
                key={ticket.code}
                className="p-5 border rounded-lg shadow-lg bg-black text-white"
              >
                <h3 className="text-lg font-bold mb-2">{ticket.name}</h3>
                <p className="text-gray-300">Code: {ticket.code}</p>
                <p className="text-gray-300">Category: {ticket.category}</p>
                <p className="text-gray-300">Price: ${ticket.price}</p>
                <p className="text-gray-300">
                  Event Date: {new Date(ticket.eventDate).toDateString()}
                </p>
                <p className="text-gray-300">
                  Remaining Quota: {ticket.remainingQuota}
                </p>

                {/* Quantity Input */}
                <input
                  type="number"
                  min="1"
                  max={ticket.remainingQuota}
                  value={bookingData[ticket.code] || ""}
                  onChange={(e) => handleQuantityChange(ticket.code, e.target.value)}
                  className="border p-2 w-full my-2 text-black bg-white"
                  placeholder="Enter quantity"
                />

                {/* Book Ticket Button */}
                <button
                  onClick={() => handleBookTicket(ticket)}
                  className={`px-4 py-2 rounded w-full transition ${
                    bookingData[ticket.code]
                      ? "bg-blue-500 hover:bg-blue-600 text-white"
                      : "bg-gray-500 text-gray-300 cursor-not-allowed"
                  }`}
                  disabled={!bookingData[ticket.code]}
                >
                  Book Ticket
                </button>
              </div>
            ))
          ) : (
            <p className="text-center text-gray-400 col-span-2">
              No tickets match your filters.
            </p>
          )}
        </motion.div>
      </AnimatePresence>

      {/* View Booked Tickets Section */}
      <section id="booked-tickets" className="mt-10">
        <h2 className="text-xl font-semibold mt-6 text-white">View Your Booked Tickets</h2>
      </section>

      {/* GUID Input */}
      <input
        type="text"
        placeholder="Enter your GUID"
        value={guid}
        onChange={(e) => setGuid(e.target.value)}
        className="border p-2 w-full my-2 text-gray"
      />
      <button
        onClick={fetchBookedTickets}
        className="bg-green-500 text-white px-4 py-2 rounded w-full hover:bg-green-600 transition"
      >
        See Booked Tickets
      </button>

      {/* Booked Tickets List */}
      <AnimatePresence mode="wait">
        {bookedTickets.map((ticket, index) => (
          <motion.div
            key={ticket.ticketCode || `booked-${index}`}
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.5 }}
            className="p-4 border rounded-lg shadow-md bg-gray-800 text-white mt-4"
          >
            <h3 className="text-lg font-bold">{ticket.ticketName}</h3>
            <p className="text-gray-300">Code: {ticket.ticketCode}</p>
            <p className="text-gray-300">Category: {ticket.category}</p>
            <p className="text-gray-300">Price: ${ticket.price}</p>
            <p className="text-gray-300">
              Event Date: {new Date(ticket.eventDate).toDateString()}
            </p>
            <p className="text-gray-300">Quantity: {ticket.quantity}</p>
            <hr className="my-2 border-gray-500" />

            <p className="text-gray-300 font-semibold">
              Total Price: ${(ticket.price * ticket.quantity).toFixed(2)}
            </p>

            {/* Revoke Ticket Input & Button */}
            <input
              type="number"
              min="1"
              max={ticket.quantity}
              value={bookingData[ticket.ticketCode] || ""}
              onChange={(e) =>
                setBookingData({ ...bookingData, [ticket.ticketCode]: e.target.value })
              }
              className="border p-2 w-full my-2 text-gray"
              placeholder="Enter quantity to revoke"
            />
            <button
              onClick={() => {
                const bookedId = bookedTicketId;
                const ticketCode = ticket.ticketCode;
                const quantity = parseInt(bookingData[ticket.ticketCode] || 0, 10);

                console.log("🛠 DEBUG → Booked Ticket ID:", bookedId);
                console.log("🛠 DEBUG → Ticket Code:", ticketCode);
                console.log("🛠 DEBUG → Quantity to revoke:", quantity);

                revokeTicket(bookedId, ticketCode, quantity);
              }}
              className="bg-red-500 text-white px-4 py-2 rounded w-full mt-2 hover:bg-red-600 transition"
            >
              Revoke Ticket
            </button>

            {/* Edit Ticket Input & Button */}
            <input
              type="number"
              min="1"
              value={editData[ticket.ticketCode] ?? ""}
              onChange={(e) =>
                setEditData({ ...editData, [ticket.ticketCode]: e.target.value })
              }
              className="border p-2 w-full my-2 text-gray"
              placeholder="Enter new quantity"
            />
            <button

              onClick={() =>{
                console.log("🛠 DEBUG → Edit Ticket Button Clicked!");
                handleEditBookedTicket(bookedTicketId, ticket.ticketCode)}}
              className="bg-yellow-500 text-white px-4 py-2 rounded w-full mt-2 hover:bg-yellow-600 transition"
            >
              Edit Ticket
            </button>
          </motion.div>
        ))}
        {message && (
        <p className="mt-4 mb-4 text-center text-lg font-semibold text-red-500">
          {message}
        </p>
        )}
      </AnimatePresence>
    </div>
  );
}
