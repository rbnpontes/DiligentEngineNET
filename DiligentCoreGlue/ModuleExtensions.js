function module_extensions_setup(module) {
    const delegate_tbl = {};
    const obj_id = [];

    function memory_copy_net_2_diligent(mem_ptr, net_buffer, size) {
        const ptr_idx = mem_ptr >> 0;
        /**
         * @type {Uint8Array}
         */
        const buffer = module.HEAPU8.subarray(ptr_idx, ptr_idx + size);
        net_buffer.copyTo(buffer, 0);
    }

    function memory_copy_diligent_2_net(mem_ptr, net_buffer, size) {
        const ptr_idx = mem_ptr >> 0;
        /**
         * @type {Uint8Array}
         */
        const buffer = module.HEAPU8.subarray(ptr_idx, ptr_idx + size);
        buffer.set(net_buffer, 0);
    }

    function delegate_register(delegate, signature) {
        const delegate_entry = { 
            call: delegate, 
            ptr: 0,
            signature,
        };
        const delegate_ptr = module.addFunction((...args)=> {
            const target_delegate = delegate_tbl[delegate_entry.ptr];
            if(!target_delegate)
                return;

            const obj_idx = object_list__push(args);
            target_delegate.call(obj_idx);
            object_list__free(obj_idx);
        }, signature);
        delegate_entry.ptr = delegate_ptr;
        delegate_tbl[delegate_ptr] = delegate_entry;
        return delegate_ptr;
    }

    function delegate_override(delegate_ptr, delegate) {
        if(!delegate_tbl[delegate_ptr])
            return;
        const delegate_entry = delegate_tbl[delegate_ptr];
        delegate_entry.call.dispose();
        delegate_entry.call = delegate;
    }

    function delegate_unregister(delegate_ptr) {
        if(!delegate_tbl[delegate_ptr])
            return;
        const entry = delegate_tbl[delegate_ptr];
        delete delegate_tbl[delegate_ptr];
        module.removeFunction(delegate_ptr);
        entry.call.dispose();
    }

    function object_list__push(obj) {
        obj_id.push(obj);
        return obj_id.length - 1;
    }

    function object_list__get(obj_id) {
        return obj_id[obj_id];
    }

    function object_list__free(obj_id) {
        if(!obj_id[obj_id])
            return;
        obj_id.splice(obj_id, 1);
    }

    function object_list__array_get(obj_id, idx) {
        if(!obj_id[obj_id])
            return;
        return obj_id[obj_id][idx];
    }

    function object_list__array_set(obj_id, idx, value) {
        if(!obj_id[obj_id])
            return;
        obj_id[obj_id][idx] = value;
    }

    const calls = {
        memory_copy_diligent_2_net,
        memory_copy_net_2_diligent,
        delegate_register,
        delegate_override,
        delegate_unregister,
        object_list__get,
        object_list__push,
        object_list__free,
        object_list__array_get,
        object_list__array_set
    };

    Object.entries(calls).forEach(([key, value]) => {
        module[key] = value;
    });
}
module_extensions_setup(Module);