<template>
    <VContainer style="padding: unset;" class="flex flex-column">
        <h2 className="text-2xl font-bold text-gray-800">Tổng quan</h2>

        <MLVbox style="overflow-y: auto;" class="mt-6 px-1">
        <!-- Dòng 1 -->
        <div className="grid grid-cols-4 gap-6 mb-6">
            <div className="bg-gray-50 rounded-xl p-6 shadow-md hover:shadow-lg transition duration-300 border-l-4 border-purple-500">
                <div className="flex justify-between h-full">
                    <div>
                        <p className="text-gray-500 text-sm">Doanh thu hôm nay</p>
                        <h3 className="text-2xl font-bold mt-1">$3,542.70</h3>
                        <p className="text-green-600 text-sm mt-1 flex items-center">
                            <span className="material-symbols-outlined text-sm mr-1">trending_up</span>
                            +12.5% from yesterday
                        </p>
                    </div>
                    <div className="bg-purple-100 rounded-full pa-3 text-purple-600">
                        <span className="material-symbols-outlined text-3xl">payments</span>
                    </div>
                </div>
            </div>

            <div className="bg-gray-50 rounded-xl p-6 shadow-md hover:shadow-lg transition duration-300 border-l-4 border-blue-500">
                <div className="flex justify-between h-full">
                    <div>
                        <p className="text-gray-500 text-sm">Số order đã hoàn thành</p>
                        <h3 className="text-2xl font-bold mt-1">87</h3>
                        <p className="text-green-600 text-sm mt-1 flex items-center">
                            <span className="material-symbols-outlined text-sm mr-1">trending_up</span>
                            +5.2% from yesterday
                        </p>
                    </div>
                    <div className="bg-blue-100 rounded-full pa-3 text-blue-600">
                        <span className="material-symbols-outlined text-3xl">receipt_long</span>
                    </div>
                </div>
            </div>

            <div className="bg-gray-50 rounded-xl p-6 shadow-md hover:shadow-lg transition duration-300 border-l-4 border-yellow-500">
                <div className="flex justify-between h-full">
                    <div>
                        <p className="text-gray-500 text-sm">Giá trị order trung bình</p>
                        <h3 className="text-2xl font-bold mt-1">$40.72</h3>
                        <p className="text-red-500 text-sm mt-1 flex items-center">
                            <span className="material-symbols-outlined text-sm mr-1">trending_down</span>
                            -2.1% from yesterday
                        </p>
                    </div>
                    <div className="bg-yellow-100 rounded-full pa-3 text-yellow-600">
                        <span className="material-symbols-outlined text-3xl">shopping_cart</span>
                    </div>
                </div>
            </div>

            <div className="bg-gray-50 rounded-xl p-6 shadow-md hover:shadow-lg transition duration-300 border-l-4 border-green-500">
                <div className="flex justify-between h-full">
                    <div>
                        <p className="text-gray-500 text-sm">Số lượng bàn hoạt động</p>
                        <h3 className="text-2xl font-bold mt-1">12/15</h3>
                        <p className="text-gray-500 text-sm mt-1 flex items-center">80% occupancy rate</p>
                    </div>
                    <div className="bg-green-100 rounded-full pa-3 text-green-600">
                        <span className="material-symbols-outlined text-3xl">table_restaurant</span>
                    </div>
                </div>
            </div>
        </div>

        <!-- Dòng 2 -->
        <div className="grid grid-cols-3 gap-6 mb-6">
            <div className="bg-gray-50 rounded-xl p-6 shadow-md hover:shadow-lg transition duration-300 col-span-2">
                <div className="flex justify-between items-center mb-4">
                    <h3 className="text-lg font-bold">Revenue Trend</h3>
                    <div className="flex gap-2">
                        <button className="px-3 py-1 text-sm bg-emerald-100 text-emerald-700 rounded-md hover:bg-primary-200 transition duration-150">
                            Day
                        </button>
                        <button className="px-3 py-1 text-sm bg-emerald-600 text-white rounded-md hover:bg-emerald-700 transition duration-150">
                            Week
                        </button>
                        <button className="px-3 py-1 text-sm bg-emerald-100 text-emerald-700 rounded-md hover:bg-primary-200 transition duration-150">
                            Month
                        </button>
                    </div>
                </div>
                <div className="w-full" style="height: 300px;">

                </div>
            </div>

            <div className="bg-gray-50 rounded-xl p-6 shadow-md hover:shadow-lg transition duration-300">
                <div className="flex justify-between items-center mb-4">
                    <h3 className="text-lg font-bold">Món ăn phổ biến</h3>
                </div>
                <div className="space-y-4">
                    <div className="flex items-center gap-3 p-2 hover:bg-gray-50 rounded-lg transition duration-150">
                        <img
                            src="https://images.unsplash.com/photo-1579871494447-9811cf80d66c?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60"
                            alt="Dragon Roll"
                            className="w-12 h-12 object-cover rounded-md"
                        />
                        <div className="flex-1">
                            <h4 className="font-medium">Dragon Roll</h4>
                            <p className="text-sm text-gray-500">Ordered 48 times</p>
                        </div>
                        <div className="text-right">
                            <p className="font-bold">$16.99</p>
                            <p className="text-xs text-green-600">+23%</p>
                        </div>
                    </div>

                    <div className="flex items-center gap-3 p-2 hover:bg-gray-50 rounded-lg transition duration-150">
                        <img
                            src="https://images.unsplash.com/photo-1553621042-f6e147245754?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60"
                            alt="Salmon Nigiri"
                            className="w-12 h-12 object-cover rounded-md"
                        />
                        <div className="flex-1">
                            <h4 className="font-medium">Salmon Nigiri</h4>
                            <p className="text-sm text-gray-500">Ordered 42 times</p>
                        </div>
                        <div className="text-right">
                            <p className="font-bold">$12.99</p>
                            <p className="text-xs text-green-600">+18%</p>
                        </div>
                    </div>

                    <div className="flex items-center gap-3 p-2 hover:bg-gray-50 rounded-lg transition duration-150">
                        <img
                            src="https://images.unsplash.com/photo-1617196034183-421b4917c92d?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60"
                            alt="Spicy Tuna Roll"
                            className="w-12 h-12 object-cover rounded-md"
                        />
                        <div className="flex-1">
                            <h4 className="font-medium">Spicy Tuna Roll</h4>
                            <p className="text-sm text-gray-500">Ordered 36 times</p>
                        </div>
                        <div className="text-right">
                            <p className="font-bold">$14.99</p>
                            <p className="text-xs text-green-600">+12%</p>
                        </div>
                    </div>

                    <div className="flex items-center gap-3 p-2 hover:bg-gray-50 rounded-lg transition duration-150">
                        <img
                            src="https://images.unsplash.com/photo-1628699011941-4a992ea1333c?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=60"
                            alt="Tempura Udon"
                            className="w-12 h-12 object-cover rounded-md"
                        />
                        <div className="flex-1">
                            <h4 className="font-medium">Tempura Udon</h4>
                            <p className="text-sm text-gray-500">Ordered 31 times</p>
                        </div>
                        <div className="text-right">
                            <p className="font-bold">$18.99</p>
                            <p className="text-xs text-red-500">-3%</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Dòng 3 -->
        <div className="grid grid-cols-2 gap-6 mb-6">
            <div className="bg-gray-50 rounded-xl p-6 shadow-md hover:shadow-lg transition duration-300">
                <div className="flex justify-between items-center mb-4">
                    <h3 className="text-lg font-bold">Recent Orders</h3>
                    <button className="text-primary-600 hover:text-primary-700 flex items-center gap-1 text-sm transition duration-150">
                        View All
                        <span className="material-symbols-outlined text-sm">chevron_right</span>
                    </button>
                </div>
                <div className="overflow-x-auto">
                    <table className="w-full text-left">
                        <thead>
                            <tr className="border-b">
                                <th className="pb-3 font-medium">Order ID</th>
                                <th className="pb-3 font-medium">Table</th>
                                <th className="pb-3 font-medium">Items</th>
                                <th className="pb-3 font-medium">Total</th>
                                <th className="pb-3 font-medium">Status</th>
                                <th className="pb-3 font-medium">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr className="border-b hover:bg-gray-50 transition duration-150">
                                <td className="py-3">#1253</td>
                                <td className="py-3">Table 7</td>
                                <td className="py-3">4 items</td>
                                <td className="py-3 font-medium">$84.50</td>
                                <td className="py-3">
                                    <span className="px-2 py-1 bg-green-100 text-green-800 rounded-full text-xs">
                                        Completed
                                    </span>
                                </td>
                                <td className="py-3">
                                    <button className="text-gray-500 hover:text-primary-600 transition duration-150 mr-2">
                                        <span className="material-symbols-outlined">visibility</span>
                                    </button>
                                    <button className="text-gray-500 hover:text-primary-600 transition duration-150">
                                        <span className="material-symbols-outlined">receipt_long</span>
                                    </button>
                                </td>
                            </tr>
                            <tr className="border-b hover:bg-gray-50 transition duration-150">
                                <td className="py-3">#1252</td>
                                <td className="py-3">Table 3</td>
                                <td className="py-3">2 items</td>
                                <td className="py-3 font-medium">$37.25</td>
                                <td className="py-3">
                                    <span className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs">
                                        In Progress
                                    </span>
                                </td>
                                <td className="py-3">
                                    <button className="text-gray-500 hover:text-primary-600 transition duration-150 mr-2">
                                        <span className="material-symbols-outlined">visibility</span>
                                    </button>
                                    <button className="text-gray-500 hover:text-primary-600 transition duration-150">
                                        <span className="material-symbols-outlined">edit</span>
                                    </button>
                                </td>
                            </tr>
                            <tr className="border-b hover:bg-gray-50 transition duration-150">
                                <td className="py-3">#1251</td>
                                <td className="py-3">Table 10</td>
                                <td className="py-3">6 items</td>
                                <td className="py-3 font-medium">$129.75</td>
                                <td className="py-3">
                                    <span className="px-2 py-1 bg-yellow-100 text-yellow-800 rounded-full text-xs">
                                        Preparing
                                    </span>
                                </td>
                                <td className="py-3">
                                    <button className="text-gray-500 hover:text-primary-600 transition duration-150 mr-2">
                                        <span className="material-symbols-outlined">visibility</span>
                                    </button>
                                    <button className="text-gray-500 hover:text-primary-600 transition duration-150">
                                        <span className="material-symbols-outlined">edit</span>
                                    </button>
                                </td>
                            </tr>
                            <tr className="hover:bg-gray-50 transition duration-150">
                                <td className="py-3">#1250</td>
                                <td className="py-3">Table 5</td>
                                <td className="py-3">3 items</td>
                                <td className="py-3 font-medium">$62.80</td>
                                <td className="py-3">
                                    <span className="px-2 py-1 bg-green-100 text-green-800 rounded-full text-xs">
                                        Completed
                                    </span>
                                </td>
                                <td className="py-3">
                                    <button className="text-gray-500 hover:text-primary-600 transition duration-150 mr-2">
                                        <span className="material-symbols-outlined">visibility</span>
                                    </button>
                                    <button className="text-gray-500 hover:text-primary-600 transition duration-150">
                                        <span className="material-symbols-outlined">receipt_long</span>
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <div className="bg-gray-50 rounded-xl p-6 shadow-md hover:shadow-lg transition duration-300">
                <div className="flex justify-between items-center mb-4">
                    <h3 className="text-lg font-bold">Reservations Today</h3>
                    <button className="text-primary-600 hover:text-primary-700 flex items-center gap-1 text-sm transition duration-150">
                        Add New
                        <span className="material-symbols-outlined text-sm">add</span>
                    </button>
                </div>
                <div className="space-y-3">
                    <div className="flex pa-3 border border-green-200 bg-green-50 rounded-lg hover:shadow-md transition duration-150">
                        <div className="flex-shrink-0 flex flex-col items-center justify-center bg-green-100 text-green-800 rounded-lg pa-3 mr-4">
                            <span className="text-xl font-bold">12</span>
                            <span className="text-xs">PM</span>
                        </div>
                        <div className="flex-1">
                            <div className="flex justify-between items-start">
                                <div>
                                    <h4 className="font-medium">Takashi Yamamoto</h4>
                                    <p className="text-sm text-gray-600">4 guests • Table 6</p>
                                </div>
                                <span className="px-2 py-1 bg-green-100 text-green-800 rounded-full text-xs">
                                    Confirmed
                                </span>
                            </div>
                            <div className="flex gap-2 mt-2">
                                <button className="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-sm">call</span>
                                    Call
                                </button>
                                <button className="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-sm">edit</span>
                                    Edit
                                </button>
                            </div>
                        </div>
                    </div>

                    <div className="flex p-3 border border-blue-200 bg-blue-50 rounded-lg hover:shadow-md transition duration-150">
                        <div className="flex-shrink-0 flex flex-col items-center justify-center bg-blue-100 text-blue-800 rounded-lg p-3 mr-4">
                            <span className="text-xl font-bold">1</span>
                            <span className="text-xs">PM</span>
                        </div>
                        <div className="flex-1">
                            <div className="flex justify-between items-start">
                                <div>
                                    <h4 className="font-medium">Sarah Johnson</h4>
                                    <p className="text-sm text-gray-600">2 guests • Table 8</p>
                                </div>
                                <span className="px-2 py-1 bg-blue-100 text-blue-800 rounded-full text-xs">
                                    Arrived
                                </span>
                            </div>
                            <div className="flex gap-2 mt-2">
                                <button className="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-sm">call</span>
                                    Call
                                </button>
                                <button className="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-sm">edit</span>
                                    Edit
                                </button>
                            </div>
                        </div>
                    </div>

                    <div className="flex p-3 border border-gray-200 bg-gray-50 rounded-lg hover:shadow-md transition duration-150">
                        <div className="flex-shrink-0 flex flex-col items-center justify-center bg-gray-100 text-gray-800 rounded-lg p-3 mr-4">
                            <span className="text-xl font-bold">6</span>
                            <span className="text-xs">PM</span>
                        </div>
                        <div className="flex-1">
                            <div className="flex justify-between items-start">
                                <div>
                                    <h4 className="font-medium">David Chen</h4>
                                    <p className="text-sm text-gray-600">6 guests • Table 12</p>
                                </div>
                                <span className="px-2 py-1 bg-gray-200 text-gray-800 rounded-full text-xs">
                                    Pending
                                </span>
                            </div>
                            <div className="flex gap-2 mt-2">
                                <button className="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-sm">call</span>
                                    Call
                                </button>
                                <button className="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-sm">edit</span>
                                    Edit
                                </button>
                                <button className="px-2 py-1 text-xs bg-primary-600 text-white rounded hover:bg-primary-700 transition duration-150 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-sm">check</span>
                                    Confirm
                                </button>
                            </div>
                        </div>
                    </div>

                    <div className="flex p-3 border border-gray-200 bg-gray-50 rounded-lg hover:shadow-md transition duration-150">
                        <div className="flex-shrink-0 flex flex-col items-center justify-center bg-gray-100 text-gray-800 rounded-lg p-3 mr-4">
                            <span className="text-xl font-bold">7</span>
                            <span className="text-xs">PM</span>
                        </div>
                        <div className="flex-1">
                            <div className="flex justify-between items-start">
                                <div>
                                    <h4 className="font-medium">Emily Rodriguez</h4>
                                    <p className="text-sm text-gray-600">3 guests • Table 9</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>  
        </MLVbox>
    </VContainer>
</template>