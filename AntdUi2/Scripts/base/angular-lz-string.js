// angular-lz-string.js
// Author: Carl Ansley <carl.ansley@gmail.com>
// Wraps lz-string library by Pieroxy

(function (angular, undefined) {
    //noinspection SpellCheckingInspection
    var base64Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    function compressToBase64(input) {
        if (input == null) return "";
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = compress(input);

        while (i < input.length * 2) {

            if (i % 2 == 0) {
                chr1 = input.charCodeAt(i / 2) >> 8;
                chr2 = input.charCodeAt(i / 2) & 255;
                if (i / 2 + 1 < input.length)
                    chr3 = input.charCodeAt(i / 2 + 1) >> 8;
                else
                    chr3 = NaN;
            } else {
                chr1 = input.charCodeAt((i - 1) / 2) & 255;
                if ((i + 1) / 2 < input.length) {
                    chr2 = input.charCodeAt((i + 1) / 2) >> 8;
                    chr3 = input.charCodeAt((i + 1) / 2) & 255;
                } else
                    chr2 = chr3 = NaN;
            }
            i += 3;

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output +
                base64Characters[enc1] + base64Characters[enc2] +
                base64Characters[enc3] + base64Characters[enc4];

        }

        return output;
    }

    function decompressFromBase64(input) {
        if (input == null) return "";
        var output = "",
            ol = 0,
            output_ = null,
            chr1, chr2, chr3,
            enc1, enc2, enc3, enc4,
            i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/=]/g, "");

        while (i < input.length) {

            enc1 = base64Characters.indexOf(input[i++]);
            enc2 = base64Characters.indexOf(input[i++]);
            enc3 = base64Characters.indexOf(input[i++]);
            enc4 = base64Characters.indexOf(input[i++]);

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            if (ol % 2 == 0) {
                output_ = chr1 << 8;

                if (enc3 != 64) {
                    output += String.fromCharCode(output_ | chr2);
                }
                if (enc4 != 64) {
                    output_ = chr3 << 8;
                }
            } else {
                output = output + String.fromCharCode(output_ | chr1);

                if (enc3 != 64) {
                    output_ = chr2 << 8;
                }
                if (enc4 != 64) {
                    output += String.fromCharCode(output_ | chr3);
                }
            }
            ol += 3;
        }

        return decompress(output);

    }

    function compressToUTF16(input) {
        if (input == null) return "";
        var output = "",
            i, c,
            current = 0,
            status = 0;

        input = compress(input);

        for (i = 0; i < input.length; i++) {
            c = input.charCodeAt(i);
            switch (status) {
                case 0:
                    output += String.fromCharCode((c >> 1) + 32);
                    current = (c & 1) << 14;
                    status++;
                    break;

                case 14:
                    output += String.fromCharCode((current + (c >> 15)) + 32, (c & 32767) + 32);
                    status = 0;
                    break;

                default:
                    output += String.fromCharCode((current + (c >> (status + 1))) + 32);
                    current = (c & ((2 << status) - 1)) << (14 - status);
                    status++;
                    break;
            }
        }

        return output + String.fromCharCode(current + 32);
    }


    function decompressFromUTF16(input) {
        if (input == null) {
            return "";
        }

        var output = "", current = 0, c;
        for (var i = 0; i < input.length; i++) {
            c = input.charCodeAt(i) - 32;
            if ((i & 15) !== 0) {
                output += String.fromCharCode(current | (c >> (15 - (i & 15))));
            }
            current = (c & ((1 << (15 - (i & 15))) - 1)) << ((i + 1) & 15);
        }

        return decompress(output);
    }


    //compress into uint8array (UCS-2 big endian format)
    function compressToUint8Array(uncompressed) {
        var compressed = compress(uncompressed);
        var buf = new Uint8Array(compressed.length * 2); // 2 bytes per character

        for (var i = 0, TotalLen = compressed.length; i < TotalLen; i++) {
            var current_value = compressed.charCodeAt(i);
            buf[i * 2] = current_value >>> 8;
            buf[i * 2 + 1] = current_value % 256;
        }
        return buf;
    }


    //decompress from uint8array (UCS-2 big endian format)
    function decompressFromUint8Array(compressed) {
        if (compressed === null || compressed === undefined) {
            return decompress(compressed);
        } else {

            var buf = new Array(compressed.length / 2); // 2 bytes per character

            for (var i = 0, TotalLen = buf.length; i < TotalLen; i++) {
                buf[i] = compressed[i * 2] * 256 + compressed[i * 2 + 1];
            }

            var result = "";
            buf.forEach(function (c) {
                result += String.fromCharCode(c);
            });
            return decompress(result);
        }
    }

    //compress into a string that is already URI encoded
    function compressToEncodedURIComponent(uncompressed) {
        return compressToBase64(uncompressed).replace(/=/g, "$").replace(/\//g, "-");
    }

    //decompress from an output of compressToEncodedURIComponent
    function decompressFromEncodedURIComponent(compressed) {
        if (compressed) compressed = compressed.replace(/$/g, "=").replace(/-/g, "/");
        return decompressFromBase64(compressed);
    }


    function writeBit(value, data) {
        data.val = (data.val << 1) | value;
        if (data.position == 15) {
            data.position = 0;
            data.string += String.fromCharCode(data.val);
            data.val = 0;
        } else {
            data.position++;
        }
    }

    function writeBits(numBits, value, data) {
        if (typeof (value) == "string")
            value = value.charCodeAt(0);
        for (var i = 0; i < numBits; i++) {
            writeBit(value & 1, data);
            value = value >> 1;
        }
    }

    function produceW(context) {
        if (context.dictionaryToCreate[context.w]) {
            if (context.w.charCodeAt(0) < 256) {
                writeBits(context.numBits, 0, context.data);
                writeBits(8, context.w, context.data);
            } else {
                writeBits(context.numBits, 1, context.data);
                writeBits(16, context.w, context.data);
            }
            decrementEnlargeIn(context);
            delete context.dictionaryToCreate[context.w];
        } else {
            writeBits(context.numBits, context.dictionary[context.w], context.data);
        }
        decrementEnlargeIn(context);
    }

    function decrementEnlargeIn(context) {
        context.enlargeIn--;
        if (context.enlargeIn == 0) {
            context.enlargeIn = Math.pow(2, context.numBits);
            context.numBits++;
        }
    }

    function compress(uncompressed) {

        if (uncompressed === null || uncompressed === undefined) {
            return '';
        }

        var context = {
            dictionary: {},
            dictionaryToCreate: {},
            c: "",
            wc: "",
            w: "",
            enlargeIn: 2, // Compensate for the first entry which should not count
            dictSize: 3,
            numBits: 2,
            result: "",
            data: { string: "", val: 0, position: 0 }
        }, i;

        for (i = 0; i < uncompressed.length; i += 1) {
            context.c = uncompressed.charAt(i);
            if (!context.dictionary[context.c]) {
                context.dictionary[context.c] = context.dictSize++;
                context.dictionaryToCreate[context.c] = true;
            }

            context.wc = context.w + context.c;
            if (context.dictionary[context.wc]) {
                context.w = context.wc;
            } else {
                produceW(context);
                // Add wc to the dictionary.
                context.dictionary[context.wc] = context.dictSize++;
                context.w = String(context.c);
            }
        }

        // Output the code for w.
        if (context.w !== "") {
            produceW(context);
        }

        // Mark the end of the stream
        writeBits(context.numBits, 2, context.data);

        // Flush the last char
        while (true) {
            context.data.val = (context.data.val << 1);
            if (context.data.position == 15) {
                context.data.string += String.fromCharCode(context.data.val);
                break;
            }
            else context.data.position++;
        }

        return context.data.string;
    }

    function readBit(data) {
        var res = data.val & data.position;
        data.position >>= 1;
        if (data.position == 0) {
            data.position = 32768;
            data.val = data.string.charCodeAt(data.index++);
        }
        return res > 0 ? 1 : 0;
    }

    function readBits(numBits, data) {
        var res = 0;
        var maxpower = Math.pow(2, numBits);
        var power = 1;
        while (power != maxpower) {
            res |= readBit(data) * power;
            power <<= 1;
        }
        return res;
    }

    function decompress(compressed) {

        if (compressed === '') {
            return null;
        }

        if (compressed === null || compressed === undefined) {
            return '';
        }

        var dictionary = {},
            next,
            enlargeIn = 4,
            dictSize = 4,
            numBits = 3,
            entry = "",
            result,
            i,
            w,
            c,
            errorCount = 0,
            data = { string: compressed, val: compressed.charCodeAt(0), position: 32768, index: 1 };

        for (i = 0; i < 3; i += 1) {
            dictionary[i] = i;
        }

        next = readBits(2, data);
        switch (next) {
            case 0:
                c = String.fromCharCode(readBits(8, data));
                break;
            case 1:
                c = String.fromCharCode(readBits(16, data));
                break;
            case 2:
                return "";
        }
        dictionary[3] = c;
        w = result = c;
        while (true) {
            c = readBits(numBits, data);

            switch (c) {
                case 0:
                    if (errorCount++ > 10000) return "Error";
                    c = String.fromCharCode(readBits(8, data));
                    dictionary[dictSize++] = c;
                    c = dictSize - 1;
                    enlargeIn--;
                    break;
                case 1:
                    c = String.fromCharCode(readBits(16, data));
                    dictionary[dictSize++] = c;
                    c = dictSize - 1;
                    enlargeIn--;
                    break;
                case 2:
                    return result;
            }

            if (enlargeIn == 0) {
                enlargeIn = Math.pow(2, numBits);
                numBits++;
            }

            if (dictionary[c]) {
                entry = dictionary[c];
            } else {
                if (c === dictSize) {
                    entry = w + w.charAt(0);
                } else {
                    return null;
                }
            }
            result += entry;

            // Add w+entry[0] to the dictionary.
            dictionary[dictSize++] = w + entry.charAt(0);
            enlargeIn--;

            w = entry;

            if (enlargeIn == 0) {
                enlargeIn = Math.pow(2, numBits);
                numBits++;
            }
        }
    }

    angular.module('lz-string', [])
        .factory('LZString', function () {
            return {
                compressToBase64: compressToBase64,
                decompressFromBase64: decompressFromBase64,
                compressToUTF16: compressToUTF16,
                decompressFromUTF16: decompressFromUTF16,
                compressToUint8Array: compressToUint8Array,
                decompressFromUint8Array: decompressFromUint8Array,
                compressToEncodedURIComponent: compressToEncodedURIComponent,
                decompressFromEncodedURIComponent: decompressFromEncodedURIComponent,
                compress: compress,
                decompress: decompress
            };
        });

    if (typeof module !== 'undefined') {
        module.exports = 'lz-string';
    }

}(angular));
